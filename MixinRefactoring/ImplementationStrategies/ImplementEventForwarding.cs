using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Environment;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Generic;
using System.Linq;

namespace MixinRefactoring
{
    /// <summary>
    /// creates source code for event forwarding (add and remove accessors)
    /// </summary>
    public class ImplementEventForwarding : ImplementMemberForwardingBase<Event>
    {
        private readonly RemoveWhitespaceVisitor _removeWhitespace = new RemoveWhitespaceVisitor();
        private readonly SyntaxTrivia _newLine = SyntaxTrivia(SyntaxKind.EndOfLineTrivia, NewLine);

        public ImplementEventForwarding(
            MixinReference mixin, 
            SemanticModel semanticModel, 
            Settings settings) : 
            base(mixin, semanticModel, settings)
        {
        }

        protected override MemberDeclarationSyntax ImplementMember(Event member)
        {
            // access to member (like: "_mixin.Event")
            var memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(Name),
                IdentifierName(member.Name));

            var accessorStatements = new List<AccessorDeclarationSyntax>
            {
                // _mixin.event += value;
                AccessorDeclaration(SyntaxKind.AddAccessorDeclaration)
                    .AddBodyStatements(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.AddAssignmentExpression,
                                memberAccess,
                                IdentifierName("value")))),

                // _mixin.event -= value;
                AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration)
                    .AddBodyStatements(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SubtractAssignmentExpression,
                                memberAccess,
                                IdentifierName("value"))))
            };
            
            // do formatting if necessary
            if (_settings.AvoidLineBreaksInProperties)
                for (var i = 0; i < accessorStatements.Count; i++)
                    accessorStatements[i] = (AccessorDeclarationSyntax)
                        _removeWhitespace.Visit(accessorStatements[i])
                        .WithAdditionalAnnotations(Formatter.Annotation);

            var accessorList = new SyntaxList<AccessorDeclarationSyntax>().AddRange(accessorStatements);

            var eventDeclarationSyntax = EventDeclaration(
               ParseTypeName(ReduceQualifiedTypeName(member.EventType)),
               member.Name)
               .WithModifiers(CreateModifiers(member))
               .WithAccessorList(AccessorList(accessorList))
               .WithLeadingTrivia(CreateComment(member.Documentation));

            return eventDeclarationSyntax;
        }
    }
}
