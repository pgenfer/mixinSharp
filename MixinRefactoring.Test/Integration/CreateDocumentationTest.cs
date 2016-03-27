using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class CreateDocumentationTest
    {
        private void ValidateCommentForMember(string mixinReferenceName)
        {
            // arrange
            var sourceCode = new SourceCode(Files.Comments);
            var childClass = sourceCode.Class(nameof(Child));
            var mixinClass = childClass.FindMixinReference(mixinReferenceName);
            var settings = new Settings(includeDocumentation: true);
            var mixinCommand = new MixinCommand(sourceCode.Semantic, mixinClass);

            // act
            var newClassDeclaration = mixinCommand.Execute(settings);
            Assert.IsTrue(
                ValidationHelpers.HasSameDocumentation(
                    newClassDeclaration, 
                    mixinCommand.Mixin));
        }

        [Test]
        public void MixinWithOneMethod_CreateDocumentation_Created()
        {
            ValidateCommentForMember("_mixinWithOneMethod");
        }

        [Test]
        public void MixinWithRemark_CreateDocumentation_Created()
        {
            ValidateCommentForMember("_mixinWithRemark");
        }

        [Test]
        public void MixinWithSeeAlso_CreateDocumentation_Created()
        {
            ValidateCommentForMember("_mixinWithSeeAlso");
        }

        [Test]
        public void MixinWithSingleLineComment_CreateDocumentation_Created()
        {
            ValidateCommentForMember("_mixinWithOneLine");
        }


        [Test]
        public void MixinWithOneProperty_CreateDocumentation_Created()
        {
            ValidateCommentForMember("_mixinWithProperty");
        }
    }
}
