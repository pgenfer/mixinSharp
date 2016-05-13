using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class InterfaceListTest
    {
        private SourceCode _sourceCode;
        private ClassFactory _factory;

        [SetUp]
        public void Setup()
        {
            _sourceCode = new SourceCode(Files.Interface);
            _factory = new ClassFactory(_sourceCode.Semantic);
        }

        [Test]
        public void MixinWithInterface_Read_OneInterface()
        {
            var mixin = _factory.Create(_sourceCode.GetTypeByName(nameof(MixinWithOneInterface)));
            Assert.AreEqual(1, mixin.Interfaces.Count());
        }

        [Test]
        public void MixinWithTwoInterfaces_Read_TwoInterfaces()
        {
            var mixin = _factory.Create(_sourceCode.GetTypeByName(nameof(MixinWithTwoInterfaces)));
            Assert.AreEqual(2, mixin.Interfaces.Count());
        }

        [Test]
        public void MixinWithDerivedInterfaces_Read_TwoInterfaces()
        {
            var mixin = _factory.Create(_sourceCode.GetTypeByName(nameof(MixinWithManyInterfaces)));
            Assert.AreEqual(2,mixin.Interfaces.Count());
        }
    }
}
