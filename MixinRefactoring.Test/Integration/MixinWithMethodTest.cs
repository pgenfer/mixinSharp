using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class MixinWithMethodTest
    {
        [Test]
        public void MixinWithMethod_Include_MethodsIncluded()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClass = sourceCode.Class("Person");
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            child.Include(mixin);

            Assert.AreEqual(child.Members.Count(), mixin.Services.Count());
            foreach (var service in mixin.Services)
                Assert.AreEqual(1, child.Members.Count(x => x.Name == service.Name));
        }

        // TODO: Ensure that method parameters are also included
        // TODO: Ensure that a method is not generated when the same method is already in the child
        // TODO: Ensure that a method is generated when the same method with a different signature is in the child
        // TODO: Check base class handling: What happens if a method is already in the base class of the child / mixin?
        // TODO: ignore static methods
    }
}
