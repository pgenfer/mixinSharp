using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    public class ImplementPropertyForwarding : ImplementMemberForwardingBase<Property>
    {
        public ImplementPropertyForwarding(string mixinReferenceName, SemanticModel semanticModel, Settings settings): base (mixinReferenceName, semanticModel, settings)
        {
        }

        protected override MemberDeclarationSyntax ImplementMember(Property member)
        {
            // access to member (like: "_mixin.Property")
            var memberAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Name), IdentifierName(member.Name));
            // in case only getter => use expression body
            if (member.IsReadOnly)
            {
                return PropertyDeclaration(ParseTypeName(ReduceQualifiedTypeName(member.Type)), member.Name).WithExpressionBody(ArrowExpressionClause(memberAccess)).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
            }

            // otherwise create getter and/or setter bodies
            var getStatement = member.HasGetter ? AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).AddBodyStatements(ReturnStatement(memberAccess)) : null;
            var setStatement = member.HasSetter ? AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).AddBodyStatements(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, memberAccess, IdentifierName("value")))) : null;
            var accessorList = new SyntaxList<AccessorDeclarationSyntax>();
            // store statements in accessorlist 
            if (getStatement != null)
                accessorList = accessorList.Add(getStatement);
            if (setStatement != null)
                accessorList = accessorList.Add(setStatement);
            var propertyDeclaration = PropertyDeclaration(ParseTypeName(ReduceQualifiedTypeName(member.Type)), member.Name).WithModifiers(CreateModifiers(member)).WithAccessorList(AccessorList(accessorList));
            return propertyDeclaration;
        }
    }
}