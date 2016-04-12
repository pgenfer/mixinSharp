using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace MixinRefactoring
{
    [TestFixture]
    public class MixinCommandUnitTest
    {
        private ClassWithSourceCode _child;
        private MixinReference _mixin;
        private MixinCommand _command;
        private readonly Settings _settings = new Settings(injectMixins:true);

        [SetUp]
        public void Setup()
        {
            _child = Substitute.For<ClassWithSourceCode>();
            _mixin = Substitute.For<MixinReference>();
            _mixin.Name.Returns(x => "_mixin");

            _command = new MixinCommand(_child, _mixin);
        }


        [Test]
        public void NoSetting_DontExtendConstructor()
        {
           Assert.IsFalse(_command.NeedsConstructorExtension(null));
        }

        [Test]
        public void SettingWithoutOption_DontExtendConstructor()
        {
            Assert.IsFalse(_command.NeedsConstructorExtension(new Settings()));
        }

        [Test]
        public void NoConstructor_ExtendConstructor()
        {
            _child.HasConstructor.Returns(x => false);
            Assert.IsTrue(_command.NeedsConstructorExtension(_settings));
            
        }

        [Test]
        public void EmptyConstructor_ExtendConstructor()
        {
            _child.HasConstructor.Returns(x => true);
            _child.AllConstructorsHaveParameter("mixin").Returns(false);
            Assert.IsTrue(_command.NeedsConstructorExtension(_settings));
        }

        [Test]
        public void ConstructorWithMixin_DontExtendConstructor()
        {
            _child.HasConstructor.Returns(x => true);
            _child.AllConstructorsHaveParameter("mixin").Returns(true);
            Assert.IsFalse(_command.NeedsConstructorExtension(_settings));
        }
    }
}
