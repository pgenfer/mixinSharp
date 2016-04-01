using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Environment;

namespace MixinRefactoring
{
    /// <summary>
    /// class that generates the syntax nodes needed
    /// for forwarding member calls to the mixin reference.
    /// 
    /// </summary>
    public class IncludeMixinSyntaxWriter : CSharpSyntaxRewriter
    {
        private readonly IEnumerable<Member> _members;
        private readonly string _name;
        private readonly SemanticModel _semantic;
        private readonly Settings _settings;
        public IncludeMixinSyntaxWriter(
            IEnumerable<Member> membersToImplement, 
            string mixinReferenceName, 
            SemanticModel semanticModel, 
            Settings settings = null)
        {
            _members = membersToImplement;
            _name = mixinReferenceName;
            _semantic = semanticModel;
            _settings = settings ?? new Settings();
        }

        /// <summary>
        /// method is virtual, so derived classes can override it to create
        /// different strategies (e.g. for testing)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="semantic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected virtual Dictionary<Type, IImplementMemberForwarding> CreateStrategies(
            string name, SemanticModel semantic, Settings settings)
        {
            var implementationStrategies = new Dictionary<Type, IImplementMemberForwarding>
            {
                [typeof(Method)] = new ImplementMethodForwarding(_name, _semantic, _settings),
                [typeof(IndexerProperty)] = new ImplementIndexerForwarding(_name, _semantic, _settings),
                [typeof(Property)] = new ImplementPropertyForwarding(_name, _semantic, _settings)
            };
            return implementationStrategies;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            // currently, only three types of member forwardings are possible,
            // there is a strategy for every forwarding implementation
            var implementationStrategies = CreateStrategies(_name, _semantic, _settings);

            // needed to evaluate whether type names can be reduced (depends on the using statements in the file)
            var positionOfClassInSourceFile = classDeclaration.GetLocation().SourceSpan.Start;
            // TODO: check here if a strategy is available? But can there be a member without a strategy at all? <= maybe events?
            var membersToAdd = _members
                .Select(x => implementationStrategies[x.GetType()].ImplementMember(x, positionOfClassInSourceFile))
                .Where(x => x != null).ToArray();

            // add regions if there is something to generate
            if (_settings.CreateRegions)
            {
                var regionCaption = $" mixin {_name}";
                // if a region with the same name already exists
                // insert the new members into this region
                if (classDeclaration.HasRegion(regionCaption))
                {
                    var newClassDeclaration = classDeclaration.AddMembersIntoRegion(
                        regionCaption, membersToAdd);
                    return newClassDeclaration;
                }
                else // otherwise wrap a new region block around the members
                {
                    // add new region block around the members
                    membersToAdd.AddRegionAround(regionCaption);
                }
            }

            // return a new class node with the additional members
            // problem with AddMembers is that it adds the new code
            // after the last syntax node, so when a region exists in the class
            // the code will be added before the region end, this leads to the bug
            // https://github.com/pgenfer/mixinSharp/issues/9
            // where the newly created region is nested into the old one.
            // a solution is to ensure that the members are added after any endregion directive

            // check if there is an end region in the file
            var lastEndRegion = classDeclaration.GetLastElementInClass<EndRegionDirectiveTriviaSyntax>();
            // only interesting if there is an end region directive at all
            if(lastEndRegion != null)
            {
                var lastSyntaxNode = classDeclaration.GetLastElementInClass<SyntaxNode>(false);
                if (lastSyntaxNode != lastEndRegion && lastSyntaxNode.SpanStart < lastEndRegion.Span.End)
                {
                    // special case here: there is an end region directive at the end of the class
                    // so we must add our members AFTER this endregion (by removing it and adding it before the first member)
                    var newClassDeclaration = classDeclaration.RemoveNode(lastEndRegion, SyntaxRemoveOptions.AddElasticMarker);
                    if (membersToAdd.Length > 0)
                    {
                        membersToAdd[0] = membersToAdd[0].WithLeadingTrivia(
                            new SyntaxTriviaList()
                            .Add(Trivia(EndRegionDirectiveTrivia(true)))
                            .Add(EndOfLine(NewLine))
                            .AddRange(membersToAdd[0].GetLeadingTrivia()));
                        return newClassDeclaration.AddMembers(membersToAdd);
                    }
                }
            }

            return classDeclaration.AddMembers(membersToAdd);
        }       
    }
}