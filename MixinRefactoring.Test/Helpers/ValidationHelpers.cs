using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    public static class ValidationHelpers
    {
        /// <summary>
        /// checks if the documentation from the class member is the same as the one
        /// from the mixin member. 
        /// This only works if both have only one member
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="mixin"></param>
        /// <returns>true if member documentation is identical, false otherwise</returns>
        public static bool HasSameDocumentation(SyntaxNode classDeclaration, MixinReference mixin)
        {
            var documentationFromChild = classDeclaration
               .DescendantNodes(descendIntoTrivia: true)
               .Last(x => x is DocumentationCommentTriviaSyntax)
               .GetText().ToString();
            // get generated documentation from mixin
            var documentationFromMixin = 
                mixin
                .Class
                .MembersFromThisAndBase
                .Single()
                .Documentation
                .ToString();
            return documentationFromChild == documentationFromMixin;
        }

        /// <summary>
        /// checks if a property with the given name is enclosed
        /// by a region with the given name
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="regionName"></param>
        /// <param name="propertyName"></param>
        /// <returns>true if there is a property between the region,
        /// false otherwise</returns>
        public static bool IsPropertyBetweenRegion(
            ClassDeclarationSyntax classDeclaration, 
            string regionName,
            string propertyName)
        {
            // assert: the method must be between the region
            // get begin and end region
            var beginRegion = classDeclaration.FindRegionByText(regionName);
            var endRegion = classDeclaration.FindEndRegion(regionName);
            // get all nodes between the regions
            var span = new TextSpan(beginRegion.Span.End, endRegion.Span.Start);
            var nodesBetweenRegion = classDeclaration.DescendantNodes(span);
            // check that a property declaration for a "Name" property is there
            var isPropertyBetweenRegion = nodesBetweenRegion
                .OfType<PropertyDeclarationSyntax>()
                .Any(x => x.Identifier.ToString() == propertyName);
            return isPropertyBetweenRegion;
        }
    }
}
