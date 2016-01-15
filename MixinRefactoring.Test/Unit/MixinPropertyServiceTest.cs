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
    public class MixinPropertyServiceTest
    {
        [Test(Description=
            "Ensure that double dispatching between service and implemented service works.")]
        public void PropertyService_IsNotImplementedByMethodImplementation()
        {
            var methodServiceImplementation = Substitute.For<MethodServiceBase>("DummyMethod");
            var propertySymbolDummy = Substitute.For<IPropertySymbol>();
            var propertyService = new PropertyService(propertySymbolDummy);

            var isImplementedBy = propertyService.IsImplementedBy(methodServiceImplementation);

            Assert.IsFalse(isImplementedBy);
        }

        [Test(Description =
            "Ensure that equality method is called correctly when two services are compared with each other")]
        public void PropertyService_IsImplemented_CallsIsEqual()
        {
            // Arrange
            var propertySymbolDummy = Substitute.For<IPropertySymbol>();
            var propertyService = Substitute.For<PropertyService>(propertySymbolDummy);
            var propertyImplementation = Substitute.For<PropertyServiceBase>("DummyProperty");
            // Act
            propertyService.IsImplementedBy(propertyImplementation);
            // Assert => IsEqual was called with the correct parameter
            propertyService.Received().IsEqual(propertyImplementation);
            Assert.True(true);            
        }
    }
}
