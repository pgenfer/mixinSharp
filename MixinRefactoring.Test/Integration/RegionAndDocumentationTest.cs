using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MixinRefactoring.Test.ValidationHelpers;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class RegionAndDocumentationTest
    {
        [Test]
        public void AddDocumentationAndCreateRegion_Created()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Comments);
            var childClass = sourceCode.Class(nameof(Child));
            var mixinClass = childClass.FindMixinReference("_mixinWithProperty");
            var settings = new Settings(includeDocumentation: true,createRegions: true);
            var mixinCommand = new MixinCommand(sourceCode.Semantic, mixinClass);

            // act
            var newClassDeclaration = mixinCommand.Execute(sourceCode.Semantic,settings);

            // assert: there should be a region and a documentation
            var isPropertyBetweenRegion = IsPropertyBetweenRegion(
                newClassDeclaration, 
                "mixin _mixinWithProperty",
                "Property");
            var hasSameDocumentation = HasSameDocumentation(newClassDeclaration, mixinCommand.Mixin);
            Assert.IsTrue(isPropertyBetweenRegion);
            Assert.IsTrue(hasSameDocumentation);
        }
    }
}
