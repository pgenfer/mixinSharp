using MixinRefactoring;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring.Test
{
    public class CreateMixinFromFieldDeclarationCommandTest : IntegrationTestBase
    {
        [TestDescription(
            @"Checks that mixins cannot be created from field declarations 
            with system types (e.g. 'int _mixin' will not work)")]
        public void ClassWithNativeFields_CanExecuteMixinCommand_CannotExecute()
        {
            WithSourceFiles(Files.Person);
            var person = CreateClass(nameof(PersonWithNativeTypes));
            var fieldDeclarations = person.SourceCode.DescendantNodes().OfType<FieldDeclarationSyntax>();

            var mixinFactory = new MixinReferenceFactory(Semantic);

            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var mixin = mixinFactory.Create(fieldDeclaration);
                var mixinCommand = new IncludeMixinCommand(mixin);
                Assert.IsFalse(mixinCommand.CanExecute(person));
            }
        }

        [TestDescription("command should not execute if mixin is not valid")]
        public void CreateMixinFromFieldDeclarationCommand_NoMixin()
        {
            var command = new CreateMixinFromFieldDeclarationCommand(null);
            var childClass = NSubstitute.Substitute.For<ClassWithSourceCode>();

            Assert.False(command.CanExecute(childClass));
        }
    }
}
