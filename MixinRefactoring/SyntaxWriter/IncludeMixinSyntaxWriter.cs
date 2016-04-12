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
        private readonly MixinReference _mixin;
        private readonly SemanticModel _semantic;
        private readonly Settings _settings;
    
        public IncludeMixinSyntaxWriter(
            IEnumerable<Member> membersToImplement, 
            MixinReference mixin, 
            SemanticModel semanticModel, 
            Settings settings = null)
        {
            _members = membersToImplement;
            _mixin = mixin;
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
                [typeof(Method)] = new ImplementMethodForwarding(name, _semantic, _settings),
                [typeof(IndexerProperty)] = new ImplementIndexerForwarding(name, _semantic, _settings),
                [typeof(Property)] = new ImplementPropertyForwarding(name, _semantic, _settings)
            };
            return implementationStrategies;
        }

        /// <summary>
        /// creates a new class declaration where members are delegated to the mixin
        /// reference
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns></returns>
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            var mixinName = _mixin.Name;
            // currently, only three types of member forwardings are possible,
            // there is a strategy for every forwarding implementation
            var implementationStrategies = CreateStrategies(mixinName, _semantic, _settings);
            // needed to evaluate whether type names can be reduced (depends on the using statements in the file)
            var positionOfClassInSourceFile = classDeclaration.GetLocation().SourceSpan.Start;
           
            // generate the members that should be implemented            
            var membersToAdd = _members
                .Select(x => implementationStrategies[x.GetType()].ImplementMember(x, positionOfClassInSourceFile))
                .Where(x => x != null).ToArray();

            // add regions if there is something to generate
            if (_settings.CreateRegions)
            {
                var regionCaption = $" mixin {mixinName}";
                // if there is already a region, add members to this one,
                // otherwise create a new one
                if (classDeclaration.HasRegion(regionCaption))
                    return classDeclaration.AddMembersIntoRegion(regionCaption, membersToAdd);
                else
                    membersToAdd.AddRegionAround(regionCaption);
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
                    if (membersToAdd.Length > 0)
                    {
                        var newClassDeclaration = classDeclaration.RemoveNode(lastEndRegion, SyntaxRemoveOptions.AddElasticMarker);
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