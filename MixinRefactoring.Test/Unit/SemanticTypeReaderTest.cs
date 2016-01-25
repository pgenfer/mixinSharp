using NUnit.Framework;
using NSubstitute;

using Microsoft.CodeAnalysis;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class SemanticTypeReaderTest
    {
        /// <summary>
        /// test class that tracks access to 
        /// the dynamic dispatched methods
        /// </summary>
        public class SemanticTypeReaderTestDummy : SemanticTypeReaderBase
        {
            public bool ReadMethodWasCalled { get; private set; }
            public bool ReadPropertyWasCalled { get; private set; }
            public bool ReadParameterWasCalled { get; private set; }
            protected override void ReadSymbol(IMethodSymbol methodSymbol)
            {
                ReadMethodWasCalled = true;
            }
            protected override void ReadSymbol(IParameterSymbol parameter)
            {
                ReadParameterWasCalled = true;
            }
            protected override void ReadSymbol(IPropertySymbol propertySymbol)
            {
                ReadPropertyWasCalled = true;
            }
        }

        [Test]
        public void VisitProperty_ReadPropertyCalled()
        {
            // arrange
            var typeReader = new SemanticTypeReaderTestDummy();
            var propertySymbol = Substitute.For<IPropertySymbol>();
            // act
            typeReader.VisitSymbol(propertySymbol);
            // assert
            Assert.IsTrue(typeReader.ReadPropertyWasCalled);
        }

        [Test]
        public void VisitMethod_ReadMethodCalled()
        {
            // arrange
            var typeReader = new SemanticTypeReaderTestDummy();
            var methodSymbol = Substitute.For<IMethodSymbol>();
            // act
            typeReader.VisitSymbol(methodSymbol);
            // assert
            Assert.IsTrue(typeReader.ReadMethodWasCalled);
        }

        [Test]
        public void VisitParameter_ReadParameterCalled()
        {
            // arrange
            var typeReader = new SemanticTypeReaderTestDummy();
            var parameterSymbol = Substitute.For<IParameterSymbol>();
            // act
            typeReader.VisitSymbol(parameterSymbol);
            // assert
            Assert.IsTrue(typeReader.ReadParameterWasCalled);
        }
    }
}
