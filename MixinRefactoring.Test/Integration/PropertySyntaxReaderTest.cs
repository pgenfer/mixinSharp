using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class PropertySyntaxReaderTest
    {
        [Test]
        public void ClassWithProperty_Read_PropertyRead()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode("Person.cs");
            var personClass = sourceCode.Class("PersonWithName");

            var propertyList = new PropertyList();

            var propertySyntaxReader = new PropertySyntaxReader(propertyList,sourceCode.Semantic);
            propertySyntaxReader.Visit(personClass);

            Assert.AreEqual(1, propertyList.Count);
            Assert.AreEqual("Name", propertyList[0].Name);
        }
    }
}
