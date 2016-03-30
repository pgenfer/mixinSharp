using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class ClassDeclarationExtension
    {
        /// <summary>
        /// checks if the class declaration contains
        /// a region with the given region caption
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="regionText"></param>
        /// <returns></returns>
        public static bool HasRegion(this ClassDeclarationSyntax classDeclaration,string regionText)
        {
            return classDeclaration.FindRegionByText(regionText) != null;
        }

        public static RegionDirectiveTriviaSyntax FindRegionByText(
            this ClassDeclarationSyntax classDeclaration, string regionText)
        {
            // seems like a special approach is needed because normally, trivia nodes are skipped when traversing
            // see here:
            // https://joshvarty.wordpress.com/2015/01/21/lrn-quick-tips-working-with-regions/
            return classDeclaration
                .DescendantNodes(descendIntoTrivia: true)
                .OfType<RegionDirectiveTriviaSyntax>()
                .FirstOrDefault(x => x.GetText().ToString().Contains(regionText));
        }

        /// <summary>
        /// adds the given members at the last position within
        /// the region block with the given name
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="regionName"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static ClassDeclarationSyntax AddMembersIntoRegion(
            this ClassDeclarationSyntax classDeclaration,
            string regionName,
            IEnumerable<MemberDeclarationSyntax> members)
        {
            var beginRegion = classDeclaration.FindRegionByText(regionName);
            var endRegion = classDeclaration.FindEndRegion(regionName);
            if (beginRegion != null && endRegion != null)
                return classDeclaration.AddMembersAtRegionEnd(
                    beginRegion, 
                    endRegion, 
                    regionName,
                    members);
            // no region => add members directly
            return classDeclaration.AddMembers(members.ToArray());
        }

        /// <summary>
        /// searches the end of the given region block within
        /// the class declaration. The region block is
        /// specified by the text in the regions headline
        /// </summary>
        /// <param name="classDeclaration">class that contains the region block</param>
        /// <param name="regionText">text in regions headline (without #region token)</param>
        /// <returns>endregion directive or null if not found</returns>
        public static EndRegionDirectiveTriviaSyntax FindEndRegion(
            this ClassDeclarationSyntax classDeclaration,
            string regionText)
        {
            // store all begin and end region blocks in one list
            // and order them by their appearence in code
            IEnumerable<DirectiveTriviaSyntax> regions =
                    classDeclaration
                    .DescendantNodes(descendIntoTrivia: true)
                    .OfType<RegionDirectiveTriviaSyntax>()
                    .ToList();
            regions = regions.Concat(
                classDeclaration
                .DescendantNodes(descendIntoTrivia: true)
                .OfType<EndRegionDirectiveTriviaSyntax>())
                .OrderBy(x => x.SpanStart);
            // there is no way in roslyn to determine to 
            // which begin region an end region belongs,
            // so we count begin and endregions here to match them
            var regionCounter = 0;
            var regionWithText = 0;
            foreach (var region in regions)
            {
                var beginRegion = region as RegionDirectiveTriviaSyntax;
                if (beginRegion != null)
                {
                    regionCounter++;
                    if (beginRegion.GetText().ToString().Contains(regionText))
                        regionWithText = regionCounter;
                }
                var endRegion = region as EndRegionDirectiveTriviaSyntax;
                if (endRegion != null)
                {
                    if(regionCounter == regionWithText)
                        return endRegion;
                    // endregion found decrease the nesting counter
                    regionCounter--;
                }
            }
            return null;
        }

        /// <summary>
        /// adds the given members
        /// at the end of the given region block
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="region"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        private static ClassDeclarationSyntax AddMembersAtRegionEnd(
            this ClassDeclarationSyntax classDeclaration,
            RegionDirectiveTriviaSyntax beginRegion,
            EndRegionDirectiveTriviaSyntax endRegion,
            string regionName,
            IEnumerable<SyntaxNode> members)
        {
            // search the last node in the region
            // but the node must be in the region, not before it
            var lastSyntaxNodeForEndRegion =
                classDeclaration
                .ChildNodes()
                .OrderBy(x => x.SpanStart)
                .LastOrDefault(x => 
                x.Span.Start >= beginRegion.Span.End &&
                x.Span.End < endRegion.SpanStart);
            if(lastSyntaxNodeForEndRegion != null)
            {
                return classDeclaration.InsertNodesAfter(
                    lastSyntaxNodeForEndRegion, members);
            }
            // empty region, because there is no node
            // after which we could add our new members,
            // we remove the regions and recreate them      
            classDeclaration = classDeclaration
                .RemoveNodes(new SyntaxNode[] { beginRegion, endRegion }, 
                SyntaxRemoveOptions.AddElasticMarker);
            // surround members with region
            var memberArray = members
                .OfType<MemberDeclarationSyntax>()
                .ToArray()
                .AddRegionAround(regionName);
            // add to class declaration
            classDeclaration = classDeclaration.AddMembers(memberArray);
            return classDeclaration;
        }

        /// <summary>
        /// returns the last syntax node of the specific type in the file
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns>the last syntax node in the file or null
        /// if an element of this type does not exist in the file</returns>
        public static T GetLastElementInClass<T>(this ClassDeclarationSyntax classDeclaration, bool searchTrivia=true) 
            where T : SyntaxNode
        {
            var node =
                  classDeclaration
                    .DescendantNodes(descendIntoTrivia: searchTrivia)
                    .OfType<T>()
                    .OrderByDescending(x => x.SpanStart)
                    .FirstOrDefault();
            return node;
        }
    }
}
