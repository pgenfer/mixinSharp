using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using NSubstitute;
using NUnit.Framework;

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

            var typeSymbol = Substitute.For<ITypeSymbol>();
            var mixin = Substitute.For<MixinReference>();
            mixin.Name.Returns("_mixin");
            var property = new Property("Name", typeSymbol, true, true);
            
            var implementPropertyStrategy = new ImplementPropertyForwarding(
               mixin, Semantic, new Settings());

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

            var typeSymbol = Substitute.For<ITypeSymbol>();
            var mixin = Substitute.For<MixinReference>();
            mixin.Name.Returns("_mixin");

            var property = new Property("Name", typeSymbol, true, true);

            var implementPropertyStrategy = new ImplementPropertyForwarding(
                mixin, Semantic, new Settings(avoidLineBreaksInProperties:false));

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

        [TestDescription(
            @"Validates issue https://github.com/pgenfer/mixinSharp/issues/21.
              Internal properties which are accessible should be generated")]
        public void InternalProperty_ImplementProperty_PropertyImplemented()
        {
            WithSourceFiles(Files.Mixin);
            var mixin = CreateMixinReference("mixin", nameof(MixinWithInternalProperty));

            var implementPropertyStrategy = new ImplementPropertyForwarding(
                mixin, Semantic, new Settings());
            var propertyDeclaration = 
                implementPropertyStrategy.ImplementMember(mixin.Class.Properties.Single(), 0);

            Assert.IsNotNull(propertyDeclaration
                .DescendantNodes()
                .OfType<AccessorDeclarationSyntax>()
                .Single(x => x.Kind() == SyntaxKind.GetAccessorDeclaration));
            Assert.IsNotNull(propertyDeclaration
                .DescendantNodes()
                .OfType<AccessorDeclarationSyntax>()
                .Single(x => x.Kind() == SyntaxKind.SetAccessorDeclaration));
        }

        [TestDescription("Check that internal getter property from another assembly are not available")]
        public void InternalPropertyInExternalAssembly_GetterCannotBeAccessed()
        {
            WithExternalAssemblyFromType(typeof(ExternalClass));
            var mixin = CreateMixinReference("mixin", nameof(MixinWithInternalGetter));
            var implementPropertyStrategy = new ImplementPropertyForwarding(
                mixin, Semantic, new Settings());
            var propertyDeclaration = 
                implementPropertyStrategy.ImplementMember(mixin.Class.Properties.Single(), 0);

            Assert.IsNull(propertyDeclaration
               .DescendantNodes()
               .OfType<AccessorDeclarationSyntax>()
               .FirstOrDefault(x => x.Kind() == SyntaxKind.GetAccessorDeclaration));
            Assert.IsNotNull(propertyDeclaration
                .DescendantNodes()
                .OfType<AccessorDeclarationSyntax>()
                .Single(x => x.Kind() == SyntaxKind.SetAccessorDeclaration));
        }

        [TestDescription("Check that internal setter property from another assembly are not available")]
        public void InternalPropertyInExternalAssembly_SetterCannotBeAccessed()
        {
            WithExternalAssemblyFromType(typeof(ExternalClass));
            var mixin = CreateMixinReference("mixin", nameof(MixinWithInternalSetter));
            var implementPropertyStrategy = new ImplementPropertyForwarding(
                mixin, Semantic, new Settings());
            var propertyDeclaration =
                implementPropertyStrategy.ImplementMember(mixin.Class.Properties.Single(), 0);

            // if only a getter is available
            // the property will have arrow clause ("=>") syntax
            Assert.IsNotNull(propertyDeclaration
               .DescendantNodes()
               .OfType<ArrowExpressionClauseSyntax>()
               .Single());
            Assert.IsNull(propertyDeclaration
                .DescendantNodes()
                .OfType<AccessorDeclarationSyntax>()
                .FirstOrDefault(x => x.Kind() == SyntaxKind.SetAccessorDeclaration));
        }
    }
}
