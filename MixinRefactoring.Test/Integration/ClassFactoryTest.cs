using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    class ClassFactoryTest
    {
        [Test]
        public void ClassFactory_CreateFromCode_ClassCreated()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode(Files.Person);
            var personClass = sourceCode.Class(nameof(WorkingPerson));

            var classFactory = new ClassFactory(sourceCode.Semantic);
            var @class = classFactory.Create(personClass);

            Assert.AreEqual("Name", @class.Properties.Single().Name);
            Assert.AreEqual("void Work(int toolNumber)", @class.Methods.Single().ToString());
        }

        [Test]
        public void ClassFactory_CreateFromSymbol_ClassCreated()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode(Files.Person);
            var personClass = sourceCode.Class(nameof(WorkingPerson));

            var classFactory = new ClassFactory(sourceCode.Semantic);
            var classSymbol = sourceCode.Semantic.GetDeclaredSymbol(personClass);
            var @class = classFactory.Create((ITypeSymbol)classSymbol);

            Assert.AreEqual("Name", @class.Properties.Single().Name);
            Assert.AreEqual("void Work(int toolNumber)", @class.Methods.Single().ToString());
        }

        [Test]
        public void ClassWithBaseClass_CreateFromSymbol_ClassAndBaseClassCreated()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode(Files.Person);
            var personClass = sourceCode.Class(nameof(ThirdPersonClass));

            var classFactory = new ClassFactory(sourceCode.Semantic);
            var @class = classFactory.Create(personClass);

            Assert.IsFalse(@class.Properties.Any());
            Assert.AreEqual("Name", @class.BaseClass.BaseClass.Properties.Single().Name);            
        }
    }
}
