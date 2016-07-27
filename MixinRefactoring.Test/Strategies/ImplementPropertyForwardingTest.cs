using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    public class ImplementPropertyForwardingTest : IntegrationTestBase
    {
        [TestDescription(
            @"test that verifies that issue https://github.com/pgenfer/mixinSharp/issues/17
              is fixed. Body of getters and setters should not have extra line breaks")]
        public void ImplementProperty_AvoidLineBreaks()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);

            var typeSymbol = NSubstitute.Substitute.For<ITypeSymbol>();
            var property = new Property("Name", typeSymbol, true, true);

            var implementPropertyStrategy = new ImplementPropertyForwarding(
                "_mixin", Semantic, new Settings());

            var memberDeclaration = implementPropertyStrategy.ImplementMember(property, 0);

            // Assert:
            // let the formatting engine format the output source and ensure
            // that the text has only 6 lines:
            // <empty line>
            // public Name
            // { 
            //    get{...}
            //    set{...}
            // }

            var workspace = new AdhocWorkspace();
            memberDeclaration = (MemberDeclarationSyntax)Formatter.Format(memberDeclaration, workspace);
            var sourceText = SourceText.From(memberDeclaration.ToFullString());
            Assert.AreEqual(6, sourceText.Lines.Count);
        }

        [TestDescription(
            @"test that verifies that issue https://github.com/pgenfer/mixinSharp/issues/17
              is disabled via a setting parameter. Additional line breaks will be
              added in property accessors")]
        public void ImplementProperty_GenerateLineBreaks()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);

            var typeSymbol = NSubstitute.Substitute.For<ITypeSymbol>();
            var property = new Property("Name", typeSymbol, true, true);

            var implementPropertyStrategy = new ImplementPropertyForwarding(
                "_mixin", Semantic, new Settings(avoidLineBreaksInProperties:false));

            var memberDeclaration = implementPropertyStrategy.ImplementMember(property, 0);

            // Assert:
            // let the formatting engine format the output source and ensure
            // that the text has only 6 lines:
            // <empty line>
            // public Name
            // { 
            //    get
            //    {
            //        ...
            //    }
            //    <empty line>
            //    ...
            // }

            var workspace = new AdhocWorkspace();
            memberDeclaration = (MemberDeclarationSyntax)Formatter.Format(memberDeclaration, workspace);
            var sourceText = SourceText.From(memberDeclaration.ToFullString());
            Assert.AreEqual(13, sourceText.Lines.Count);
        }
    }
}
