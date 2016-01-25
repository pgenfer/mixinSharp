using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class MixinReferenceTest
    {
        [Test]
        public void ClassWithMixin_Read_CreateMixin()
        {
            var sourceCode = new SourceCode("Person.cs","Name.cs");
            var personClass = sourceCode.Class("Person");
            var mixinField = personClass.FindMixinReference("_name");

            var mixinFactory = new MixinReferenceFactory(sourceCode.Semantic);
            var mixin = mixinFactory.Create(mixinField);

            Assert.AreEqual("_name", mixin.Name);
            Assert.AreEqual(3, mixin.Class.Properties.Count());
        }

        [Test]
        public void ClassWithUnknownMixin_Read_NoMixinCreated()
        {
            var sourceCode = new SourceCode("Person.cs");
            var personClass = sourceCode.Class("Person");
            var mixinField = personClass.FindMixinReference("_name");

            var mixinFactory = new MixinReferenceFactory(sourceCode.Semantic);
            var mixin = mixinFactory.Create(mixinField);
            // Assert: type could not be resolved and null should be returned instead
            Assert.IsNull(mixin);
        }
    }
}
