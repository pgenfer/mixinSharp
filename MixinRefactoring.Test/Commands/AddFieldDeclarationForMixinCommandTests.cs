using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    public class AddFieldDeclarationForMixinCommandTests : IntegrationTestBase
    {
        [TestDescription("New mixin field can be added to child class")]
        public void CanExecute_True()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);
            var childClass = CreateClass(nameof(SimpleChildClassWithoutField));
            var mixinClass = CreateMixinReference("_mixin",nameof(SimpleMixin));

            var command = new AddFieldDeclarationForMixinCommand(mixinClass);
            Assert.True(command.CanExecute(childClass));
        }

        [TestDescription(
            @"New mixin field cannot be added to child class
              because child class already has a reference")]
        public void CanExecute_False()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);
            var childClass = CreateClass(nameof(SimpleChildClass));
            var mixinClass = CreateMixinReference("_mixin", nameof(SimpleMixin));

            var command = new AddFieldDeclarationForMixinCommand(mixinClass);
            Assert.False(command.CanExecute(childClass));
        }

        [TestDescription("Add mixin field to child class")]
        public void Execute_AddFieldReference()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);
            var childClass = CreateClass(nameof(SimpleChildClassWithoutField));
            var mixin = CreateMixinReference("_mixin", nameof(SimpleMixin));

            var command = new AddFieldDeclarationForMixinCommand(mixin);
            var newClassDeclaration = command.Execute(childClass.SourceCode, Semantic);
            var mixinField = newClassDeclaration.FindMixinReference("_mixin");

            Assert.NotNull(mixinField);
        }

        // TODO: check if mixin object is also created if possible
    }
}