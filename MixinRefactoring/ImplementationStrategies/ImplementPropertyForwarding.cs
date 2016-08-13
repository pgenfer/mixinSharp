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
    public class ImplementPropertyForwarding : ImplementMemberForwardingBase<Property>
    {
        private readonly RemoveWhitespaceVisitor _removeWhitespace = new RemoveWhitespaceVisitor();
        private bool _first = true;

        public ImplementPropertyForwarding(
            MixinReference mixin, 
            SemanticModel semanticModel, 
            Settings settings): 
            base (mixin, semanticModel, settings)
        {
        }

        private readonly SyntaxTrivia _newLine = SyntaxTrivia(SyntaxKind.EndOfLineTrivia, NewLine);

        /// <summary>
        /// method checks if the accessors of this property
        /// can be reached.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>A tuple with two boolean flags.
        /// The first boolean determines whether the setter is available,
        /// the second flag says if the getter can be reached.</returns>
        public Tuple<bool, bool> PropertyAccessorsCanBeReached(Property property)
        {
            var setterAccessible = property.HasSetter;
            var getterAccessible = property.HasGetter;

            if (property.IsGetterInternal || property.IsSetterInternal)
            {
                var propertySymbol = _mixin.Class.TypeSymbol
                    .GetMembers(property.Name)
                    .OfType<IPropertySymbol>()
                    .FirstOrDefault();
                if (propertySymbol != null)
                {
                    // accessor can be reached if there is a setter and
                    // this setter is either not internal or it is internal
                    // and accessable
                    setterAccessible = 
                        property.HasSetter && (!property.IsSetterInternal || 
                        _semantic.IsAccessible(_classDeclarationPosition,propertySymbol.SetMethod));
                    getterAccessible = 
                        property.HasGetter && (!property.IsGetterInternal ||
                        _semantic.IsAccessible(_classDeclarationPosition,propertySymbol.GetMethod));
                }
            }
            return Tuple.Create(setterAccessible, getterAccessible); 
        }

        protected override MemberDeclarationSyntax ImplementMember(Property member)
        {
            var propertyAccess = PropertyAccessorsCanBeReached(member);
            var hasSetter = propertyAccess.Item1;
            var hasGetter = propertyAccess.Item2;
            var isReadOnly = hasGetter && !hasSetter;
            
            // access to member (like: "_mixin.Property")
            var memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, 
                IdentifierName(Name), 
                IdentifierName(member.Name));

            PropertyDeclarationSyntax propertyDeclaration = null;

            // in case only getter => use expression body
            if (isReadOnly)
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
                if (hasGetter)
                    accessorStatements.Add(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .AddBodyStatements(ReturnStatement(memberAccess)));
                // generate setter
                if(hasSetter)
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