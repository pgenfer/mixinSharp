using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    /// <summary>
    /// class for generating forwarding code for indexers
    /// </summary>
    public class ImplementIndexerForwarding : ImplementMemberForwardingBase<IndexerProperty>
    {
        public ImplementIndexerForwarding(string mixinReferenceName, SemanticModel semanticModel, Settings settings): base (mixinReferenceName, semanticModel, settings)
        {
        }

        protected override MemberDeclarationSyntax ImplementMember(IndexerProperty member)
        {
            var parameters = ImplementParameters(member.Parameters);
            // indexer access to member, like "_collection[index]
            var memberAccess = ElementAccessExpression(IdentifierName(Name)).AddArgumentListArguments(member.Parameters.Select(x => Argument(IdentifierName(x.Name))).ToArray());
            // in case only getter => use expression body
            if (member.IsReadOnly)
            {
                return IndexerDeclaration(ParseTypeName(member.Type.ToString())).WithParameterList(BracketedParameterList(SeparatedList(parameters))).WithExpressionBody(ArrowExpressionClause(memberAccess)).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)).WithModifiers(CreateModifiers(member));
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
            var propertyDeclaration = IndexerDeclaration(ParseTypeName(ReduceQualifiedTypeName(member.Type)))
                .WithParameterList(BracketedParameterList(SeparatedList(parameters)))
                .WithModifiers(CreateModifiers(member))
                .WithAccessorList(AccessorList(accessorList))
                .WithLeadingTrivia(CreateComment(member.Documentation));
            return propertyDeclaration;
        }
    }
}