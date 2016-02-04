using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

// TODO: add using SyntaxFactory here

namespace MixinRefactoring
{
    public class IncludeMixinSyntaxWriter : CSharpSyntaxRewriter
    {
        private readonly IEnumerable<Member> _members;
        private readonly string _name;
        public IncludeMixinSyntaxWriter(IEnumerable<Member> membersToImplement, string mixinReferenceName)
        {
            _members = membersToImplement;
            _name = mixinReferenceName;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            var membersToAdd = _members
                .Select(x => ImplementDelegation((dynamic)x))
                .Where(x => x != null).OfType<MemberDeclarationSyntax>()
                .ToArray();
            var newClassDeclaration = classDeclaration.AddMembers(membersToAdd);
            return newClassDeclaration;
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(Member member)
        {
            return null;
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(Property property)
        {
            // access to member (like: "_mixin.Property")
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, 
                SyntaxFactory.IdentifierName(_name), 
                SyntaxFactory.IdentifierName(property.Name));
            // in case only getter => use expression body
            if (property.IsReadOnly)
            {
                return SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(property.Type.ToString()), property.Name)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(memberAccess))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
            }

            // otherwise create getter and/or setter bodies
            var getStatement = property.HasGetter ? 
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .AddBodyStatements(SyntaxFactory.ReturnStatement(memberAccess)) : 
                null;
            var setStatement = property.HasSetter ?
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .AddBodyStatements(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression, memberAccess, SyntaxFactory.IdentifierName("value")))) : 
                null;
            var accessorList = new SyntaxList<AccessorDeclarationSyntax>();
            // store statements in accessorlist 
            if (getStatement != null)
                accessorList = accessorList.Add(getStatement);
             if (setStatement != null)
                accessorList = accessorList.Add(setStatement);
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.ParseTypeName(property.Type.ToString()), property.Name)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(accessorList));
            return propertyDeclaration;
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(Method method)
        {
            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            modifiers = method.IsOverrideFromObject ? 
                modifiers.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)) : 
                modifiers;

            var parameters = method.Parameters.Select(x => SyntaxFactory.Parameter(
                 new SyntaxList<AttributeListSyntax>(),
                 SyntaxFactory.TokenList(),
                 SyntaxFactory.ParseTypeName(x.Type.ToString()),
                 SyntaxFactory.Identifier(x.Name),
                 null));

            // method body should delegate call to mixin directly
            var mixinServiceInvocation =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(_name),
                        SyntaxFactory.IdentifierName(method.Name)))
                .AddArgumentListArguments(method.Parameters
                    .Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name)))
                    .ToArray());
            // will be: void method(int parameter) => _mixin.method(parameter);
            var methodDeclaration =
                SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(method.ReturnType.ToString()), method.Name)
                .AddParameterListParameters(parameters.ToArray())
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(mixinServiceInvocation))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithModifiers(modifiers);
                

            return methodDeclaration;
        }
    }
}