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
    [TestFixture]
    public class MixinCommandTest
    {
        [Test]
        public void ClassWithNativeFields_CanExecuteMixinCommand_CannotExecute()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode("Person.cs");
            var personClass = sourceCode.Class("PersonWithNativeTypes");
            var fieldDeclarations = personClass.DescendantNodes().OfType<FieldDeclarationSyntax>();

            foreach(var fieldDeclaration in fieldDeclarations)
            {
                var mixinCommand = new MixinCommand(sourceCode.Semantic, fieldDeclaration);
                Assert.IsFalse(mixinCommand.CanExecute());
            }
        }
    }
}
