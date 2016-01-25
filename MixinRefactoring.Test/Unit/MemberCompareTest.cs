using Microsoft.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class PropertyServiceBaseTest
    {
        [Test]
        public void CompareProperties_DifferentName_NotEqual()
        {
            // Arrange
            var type = Substitute.For<ITypeSymbol>();
            var property = new Property("p1", type, true, true);
            var otherProperty = new Property("p2", type, true, true);
            var comparer = new MemberComparer();
            // Act
            var equal = comparer.IsImplementationOf(property, otherProperty);
            // Assert
            Assert.IsFalse(equal);
        }

        [Test(Description =
            "When comparing properties, only the name is relevant,because the name of the property " +
            "must be unique within a type definition, the type should be ignored")]
        public void CompareProperties_SameNameDifferentTypes_Equal()
        {
            // Arrange
            var typeOfProperty = Substitute.For<ITypeSymbol>();
            var typeOfOtherProperty = Substitute.For<ITypeSymbol>();
            var property = new Property("p1", typeOfProperty, true, true);
            var otherProperty = new Property("p1", typeOfOtherProperty, true, true);
            var comparer = new MemberComparer();

            // Act
            var equal = comparer.IsImplementationOf(property, otherProperty);

            // First ensure that both types are different
            Assert.AreNotEqual(typeOfProperty, typeOfOtherProperty);
            // now ensure that the properties are the same even with different types
            Assert.IsTrue(equal);
        }

        [Test(Description=
            "When comparing properties, only the name is relevant,because the name of the property " + 
            "must be unique within a type definition, the accessibility should be ignored")]
        public void CompareProperties_SameNameDifferentAccessors_Equal()
        {
            // Arrange
            var type = Substitute.For<ITypeSymbol>();
            var property = new Property("p1", type, true, true);
            var otherProperty = new Property("p1", type, true, false);
            var comparer = new MemberComparer();
            // Act
            var equal = comparer.IsImplementationOf(property, otherProperty);
            // Assert
            Assert.IsTrue(equal);
        }
    }

    // TODO: compare methods
}
