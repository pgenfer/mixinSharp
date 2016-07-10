using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class StringExtensionTest
    {
        [Test]
        public void Convert_ClassName_FieldName()
        {
            var interfaceName = nameof(SimpleMixin);
            var result = interfaceName.ConvertTypeNameToFieldName();

            Assert.AreEqual("_simpleMixin", result);
        }

        [Test]
        public void Convert_InterfaceName_FieldName()
        {
            var interfaceName = "ISimpleMixin";
            var result = interfaceName.ConvertTypeNameToFieldName();

            Assert.AreEqual("_simpleMixin", result);
        }
    }
}