using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring.Test
{
    public class MixinWithEventTest : IntegrationTestBase
    {
        [TestDescription("Ensure that an event is generated in the child class")]
        public void ChildHasHiddenProperties_Include_PropertiesNotGenerated()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);

            var child = CreateClass(nameof(ChildClassWithEventsFromMixin));
            var mixin = CreateMixinReference(child, "_mixin");

            var includeMixin = new IncludeMixinCommand(mixin);
            var generatedClass = includeMixin.Execute(child.SourceCode, Semantic);

            Assert.IsNotEmpty(generatedClass.Members.OfType<EventDeclarationSyntax>());
        }
    }
}
