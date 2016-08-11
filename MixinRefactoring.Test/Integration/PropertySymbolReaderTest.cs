using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MixinRefactoring.Test
{
    public class PropertySymbolReaderTest : IntegrationTestBase
    {
        [TestDescription(
            @"Check that private set accessors are ignored during generation.
              See https://github.com/pgenfer/mixinSharp/issues/19")]
        public void MixinWithPrivateSetAccessor_Generate_PropertyIsGeneratedReadonly()
        {
            WithSourceFiles(Files.ChildClass,Files.Mixin);
            var child = CreateClass(nameof(ChildClassWithPrivateSetterMixin));

            var mixin = CreateMixinReference(child, "_mixin");
            var property = mixin.Class.Properties.Single();

            // assert that only the property getter is available
            Assert.IsTrue(property.IsReadOnly);
        }
    }
}
