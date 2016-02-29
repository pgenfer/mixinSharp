using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
            var parameters = parameterList.Select(x => Parameter(
                new SyntaxList<AttributeListSyntax>(),
                TokenList(),
                ParseTypeName(ReduceQualifiedTypeName(x.Type)),
                Identifier(x.Name),
                null));
            return parameters;
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(IndexerProperty property)
        {
            var parameters = ImplementParameters(property.Parameters);

            // indexer access to member, like "_collection[index]
            var memberAccess = ElementAccessExpression(
                IdentifierName(_name))
                .AddArgumentListArguments(
                    property.Parameters
                    .Select(x => Argument(IdentifierName(x.Name)))
                    .ToArray());
            // in case only getter => use expression body
            if (property.IsReadOnly)
            {
                return IndexerDeclaration(ParseTypeName(property.Type.ToString()))
                    .WithParameterList(BracketedParameterList(SeparatedList(parameters)))
                    .WithExpressionBody(ArrowExpressionClause(memberAccess))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    .WithModifiers(CreateModifiers(property));
            }

            // otherwise create getter and/or setter bodies
            var getStatement = property.HasGetter ?
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .AddBodyStatements(ReturnStatement(memberAccess)) :
                null;
            var setStatement = property.HasSetter ?
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .AddBodyStatements(ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression, memberAccess, IdentifierName("value")))) :
                null;
            var accessorList = new SyntaxList<AccessorDeclarationSyntax>();
            // store statements in accessorlist 
            if (getStatement != null)
                accessorList = accessorList.Add(getStatement);
            if (setStatement != null)
                accessorList = accessorList.Add(setStatement);
            var propertyDeclaration = IndexerDeclaration(
                ParseTypeName(ReduceQualifiedTypeName(property.Type)))
                .WithParameterList(BracketedParameterList(SeparatedList(parameters)))
                .WithModifiers(CreateModifiers(property))
                .WithAccessorList(AccessorList(accessorList));
            return propertyDeclaration;
        }

        protected virtual MemberDeclarationSyntax ImplementDelegation(Property property)
        {
            // access to member (like: "_mixin.Property")
            var memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, 
                IdentifierName(_name), 
                IdentifierName(property.Name));
            // in case only getter => use expression body
            if (property.IsReadOnly)
            {
                return PropertyDeclaration(ParseTypeName(ReduceQualifiedTypeName(property.Type)), property.Name)
                    .WithExpressionBody(ArrowExpressionClause(memberAccess))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
            }

            // otherwise create getter and/or setter bodies
            var getStatement = property.HasGetter ? 
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .AddBodyStatements(ReturnStatement(memberAccess)) : 
                null;
            var setStatement = property.HasSetter ?
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .AddBodyStatements(ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression, memberAccess, IdentifierName("value")))) : 
                null;
            var accessorList = new SyntaxList<AccessorDeclarationSyntax>();
            // store statements in accessorlist 
            if (getStatement != null)
                accessorList = accessorList.Add(getStatement);
             if (setStatement != null)
                accessorList = accessorList.Add(setStatement);
            var propertyDeclaration = PropertyDeclaration(
                ParseTypeName(ReduceQualifiedTypeName(property.Type)),
                property.Name)
                .WithModifiers(CreateModifiers(property))
                .WithAccessorList(AccessorList(accessorList));
            return propertyDeclaration;
        }

        protected SyntaxTokenList CreateModifiers(Method method)
        {
            var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
            modifiers = method.IsOverrideFromObject || method.IsOverride ?
                modifiers.Add(Token(SyntaxKind.OverrideKeyword)) :
                modifiers;
            return modifiers;
        }

        protected SyntaxTokenList CreateModifiers(Member member)
        {
            var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
            modifiers = member.IsOverride ?
                modifiers.Add(Token(SyntaxKind.OverrideKeyword)) :
                modifiers;
            return modifiers;
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
            var parameters = ImplementParameters(method.Parameters);

            // method body should delegate call to mixin directly
            var mixinServiceInvocation =
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(_name),
                        IdentifierName(method.Name)))
                .AddArgumentListArguments(method.Parameters
                    .Select(x => Argument(IdentifierName(x.Name)))
                    .ToArray());
            // will be: void method(int parameter) => _mixin.method(parameter);
            var methodDeclaration =
                MethodDeclaration(ParseTypeName(ReduceQualifiedTypeName(method.ReturnType)), method.Name)
                .AddParameterListParameters(parameters.ToArray())
                .WithExpressionBody(ArrowExpressionClause(mixinServiceInvocation))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithModifiers(CreateModifiers(method));                

            return methodDeclaration;
        }
    }
}