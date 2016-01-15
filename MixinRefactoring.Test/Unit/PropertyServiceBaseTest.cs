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
            var propertySymbol = Substitute.For<IPropertySymbol>();
            propertySymbol.Name.Returns("PropertyService1");
            var property = new PropertyService(propertySymbol);

            var otherPropertySymbol = Substitute.For<IPropertySymbol>();
            otherPropertySymbol.Name.Returns("PropertyService2");
            var other = new PropertyService(otherPropertySymbol);

            // Act
            var equal = property.IsEqual(other);
            // Assert
            Assert.IsFalse(equal);
        }

        [Test(Description =
            "When comparing properties, only the name is relevant,because the name of the property " +
            "must be unique within a type definition, the type should be ignored")]
        public void CompareProperties_SameNameDifferentTypes_Equal()
        {
            // Arrange
            var propertySymbol = Substitute.For<IPropertySymbol>();
            var typeOfProperty = Substitute.For<ITypeSymbol>();
            propertySymbol.Name.Returns("PropertyService1");
            propertySymbol.Type.Returns(typeOfProperty);
            var property = new PropertyService(propertySymbol);

            var otherProperty = Substitute.For<IPropertySymbol>();
            var typeOfOtherProperty = Substitute.For<ITypeSymbol>();
            otherProperty.Name.Returns("PropertyService1");
            otherProperty.Type.Returns(typeOfOtherProperty);
            var other = new PropertyService(otherProperty);

            // Act
            var equal = property.IsEqual(other);

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
            var propertySymbol = Substitute.For<IPropertySymbol>();
            propertySymbol.SetMethod.DeclaredAccessibility.Returns(Accessibility.Public);
            propertySymbol.GetMethod.DeclaredAccessibility.Returns(Accessibility.Public);
            propertySymbol.Name.Returns("PropertyService1");
            var property = new PropertyService(propertySymbol);

            var otherPropertySymbolOnlyGetter = Substitute.For<IPropertySymbol>();
            otherPropertySymbolOnlyGetter.Name.Returns("PropertyService1");
            otherPropertySymbolOnlyGetter.GetMethod.DeclaredAccessibility.Returns(Accessibility.Public);
            otherPropertySymbolOnlyGetter.SetMethod.Returns((IMethodSymbol)null);
            var other = new PropertyService(otherPropertySymbolOnlyGetter);

            // Act
            var equal = property.IsEqual(other);
            // Assert
            Assert.IsTrue(equal);
        }
    }
}
