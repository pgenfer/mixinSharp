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

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            Assert.AreEqual(mixer.MethodsToImplement.Count(), mixin.Class.Methods.Count());
            foreach (var service in mixin.Class.Methods)
                Assert.AreEqual(1, mixer.MethodsToImplement.Count(x => x.Name == service.Name));
        }

        [Test]
        public void MethodAlreadyImplemented_Include_MethodNotIncluded()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClass = sourceCode.Class("PersonWithWorkMethod");
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no method to implement
            Assert.IsEmpty(mixer.MethodsToImplement);
        }

        [Test]
        public void MethodImplementedWithOtherParameter_Include_MethodIncluded()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClass = sourceCode.Class("PersonWithOtherWorkMethod");
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no method to implement
            Assert.AreEqual(1,mixer.MethodsToImplement.Count(x => x.Name == "Work"));
        }

        [Test]
        public void MixinWithStaticMethod_Include_MethodNotIncluded()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClass = sourceCode.Class("PersonWithStaticMixin");
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no method to implement
            Assert.IsEmpty(mixer.MethodsToImplement);
        }

        [Test]
        public void MixinWithToString_Include_ToStringShouldBeImplemented()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClass = sourceCode.Class("PersonWithToString");
            var mixinReference = personClass.FindMixinReference("_toString");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // ToString should be in list of methods to override
            Assert.IsTrue(mixer.MethodsToImplement.Any(x => x.Name == "ToString"));
            // ToString in mixin must have override keyword
            Assert.IsTrue(mixer.MethodsToImplement.Single(x => x.Name == "ToString").IsOverrideFromObject);
        }

        // TODO: Check base class handling: What happens if a method is already in the base class of the child / mixin?
        // TODO: ignore static methods
    }
}
