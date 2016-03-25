using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class MixerTest
    {
        [Test]
        public void NoPropertiesInMixin_Mix_AllPropertiesToImplement()
        {
            var sourceCode = new SourceCode(Files.Person,Files.Name);
            var personClass = sourceCode.Class(nameof(Person));
            var mixinField = personClass.FindMixinReference("_name");

            var child = new ClassFactory(sourceCode.Semantic).Create(personClass);
            var mixin = new MixinReferenceFactory(sourceCode.Semantic).Create(mixinField);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // Assert: all properties of mixin should be implemented
            Assert.AreEqual(mixin.Class.Properties.Count(), mixer.MembersToImplement.Count());
        }

        [Test]
        public void PropertiesInMixinAndChild_Mix_OnlyMissingPropertiesToImplement()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Name);
            var personClass = sourceCode.Class(nameof(PersonWithFullName));
            var mixinField = personClass.FindMixinReference("_name");

            var child = new ClassFactory(sourceCode.Semantic).Create(personClass);
            var mixin = new MixinReferenceFactory(sourceCode.Semantic).Create(mixinField);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // Assert: all properties of mixin should be implemented
            Assert.AreEqual(2, mixer.MembersToImplement.Count());
        }

        [Test]
        public void PropertiesInBaseClass_Mix_NoPropertyToImplement()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Name);
            var personClass = sourceCode.Class(nameof(ThirdPersonClass));
            var mixinField = personClass.FindMixinReference("_name");

            var child = new ClassFactory(sourceCode.Semantic).Create(personClass);
            var mixin = new MixinReferenceFactory(sourceCode.Semantic).Create(mixinField);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // Assert: all properties of mixin should be implemented
            Assert.IsEmpty(mixer.MembersToImplement);
        }
    }
}
