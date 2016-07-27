using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Environment;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using System.Collections.Generic;
using System.Linq;

namespace MixinRefactoring
{
    public class ImplementPropertyForwarding : ImplementMemberForwardingBase<Property>
    {
        private RemoveWhitespaceVisitor _removeWhitespace = new RemoveWhitespaceVisitor();
        private bool _first = true;

        public ImplementPropertyForwarding(
            string mixinReferenceName, 
            SemanticModel semanticModel, 
            Settings settings): 
            base (mixinReferenceName, semanticModel, settings)
        {
        }

        private readonly SyntaxTrivia _newLine = SyntaxTrivia(SyntaxKind.EndOfLineTrivia, NewLine);

        protected override MemberDeclarationSyntax ImplementMember(Property member)
        {
            // access to member (like: "_mixin.Property")
            var memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, 
                IdentifierName(Name), 
                IdentifierName(member.Name));

            PropertyDeclarationSyntax propertyDeclaration = null;

            // in case only getter => use expression body
            if (member.IsReadOnly)
            {
                propertyDeclaration = PropertyDeclaration(
                    ParseTypeName(ReduceQualifiedTypeName(member.Type)),
                    member.Name)
                    .WithExpressionBody(ArrowExpressionClause(memberAccess))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
            }
            else
            {
                // store accessors for later use 
                var accessorStatements = new List<AccessorDeclarationSyntax>();

                // generate getter
                if (member.HasGetter)
                    accessorStatements.Add(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .AddBodyStatements(ReturnStatement(memberAccess)));
                // generate setter
                if(member.HasSetter)
                    accessorStatements.Add(
                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .AddBodyStatements(ExpressionStatement(AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression, memberAccess, IdentifierName("value")))));

                // do formatting if necessary
                if(_settings.AvoidLineBreaksInProperties)
                    for(var i=0;i<accessorStatements.Count;i++)
                        accessorStatements[i] = (AccessorDeclarationSyntax)
                            _removeWhitespace.Visit(accessorStatements[i])
                            .WithAdditionalAnnotations(Formatter.Annotation);

                // store accessors in correct data structure
                var accessorList = new SyntaxList<AccessorDeclarationSyntax>()
                    .AddRange(accessorStatements);
               
                propertyDeclaration = PropertyDeclaration(
                    ParseTypeName(ReduceQualifiedTypeName(member.Type)), 
                    member.Name)
                .WithModifiers(CreateModifiers(member))
                .WithAccessorList(AccessorList(accessorList))
                .WithLeadingTrivia(CreateComment(member.Documentation));
            }

            // if this is the first property generated, add an additional
            // new line in front so that there is space between the declaration
            // and the properties
            if (_first)
            {
                var trivias = new List<SyntaxTrivia> { _newLine };
                // add comments and everything else after the new line
                if (propertyDeclaration.HasLeadingTrivia)
                    trivias.AddRange(propertyDeclaration.GetLeadingTrivia());
                // recreate the node with the new trivia
                propertyDeclaration = propertyDeclaration.WithLeadingTrivia(trivias);
            }
            _first = false;
            return propertyDeclaration;
        }
    }
}