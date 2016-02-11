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
        private readonly SemanticModel _semantic;
        private int _classDeclarationPosition; // we need the position of the class in source file to reduce qualified names


        public IncludeMixinSyntaxWriter(
            IEnumerable<Member> membersToImplement, string mixinReferenceName,SemanticModel semanticModel)
        {
            _members = membersToImplement;
            _name = mixinReferenceName;
            _semantic = semanticModel;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            _classDeclarationPosition = classDeclaration.GetLocation().SourceSpan.Start;

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

        private IEnumerable<ParameterSyntax> ImplementParameters(IEnumerable<Parameter> parameterList)
        {
            var parameters = parameterList.Select(x => SyntaxFactory.Parameter(
                new SyntaxList<AttributeListSyntax>(),
                SyntaxFactory.TokenList(),
                SyntaxFactory.ParseTypeName(ReduceQualifiedTypeName(x.Type)),
                SyntaxFactory.Identifier(x.Name),
                null));
            return parameters;
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(IndexerProperty property)
        {
            var parameters = ImplementParameters(property.Parameters);

            // indexer access to member, like "_collection[index]
            var memberAccess = SyntaxFactory.ElementAccessExpression(
                SyntaxFactory.IdentifierName(_name))
                .AddArgumentListArguments(
                    property.Parameters
                    .Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name)))
                    .ToArray());
            // in case only getter => use expression body
            if (property.IsReadOnly)
            {
                return SyntaxFactory.IndexerDeclaration(SyntaxFactory.ParseTypeName(property.Type.ToString()))
                    .WithParameterList(SyntaxFactory.BracketedParameterList(SyntaxFactory.SeparatedList(parameters)))
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
            var propertyDeclaration = SyntaxFactory.IndexerDeclaration(
                SyntaxFactory.ParseTypeName(ReduceQualifiedTypeName(property.Type)))
                .WithParameterList(SyntaxFactory.BracketedParameterList(SyntaxFactory.SeparatedList(parameters)))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(accessorList));
            return propertyDeclaration;
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
                SyntaxFactory.ParseTypeName(ReduceQualifiedTypeName(property.Type)),
                property.Name)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(accessorList));
            return propertyDeclaration;
        }

        /// <summary>
        /// reduces the qualification of a type name if possible.
        /// Depends on the position of the mixin class within the source code
        /// because it has to be checked whether the types namespace
        /// is already included by a using statement.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ReduceQualifiedTypeName(ITypeSymbol type)
        {
            return type.ToMinimalDisplayString(_semantic, _classDeclarationPosition);
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(Method method)
        {
            var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            modifiers = method.IsOverrideFromObject ? 
                modifiers.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)) : 
                modifiers;

            var parameters = ImplementParameters(method.Parameters);

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
                SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(ReduceQualifiedTypeName(method.ReturnType)), method.Name)
                .AddParameterListParameters(parameters.ToArray())
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(mixinServiceInvocation))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithModifiers(modifiers);                

            return methodDeclaration;
        }
    }
}