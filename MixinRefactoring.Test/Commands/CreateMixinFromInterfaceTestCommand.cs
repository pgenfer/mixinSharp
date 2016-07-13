using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring.Test
{
    public class CreateMixinFromBaseClassTest : IntegrationTestBase
    {
        [TestDescription(
            @"adds a mixin to a child class via its interface in the child's base list")]
        public void CreateMixinFromBaseClass()
        {
            WithSourceFiles(Files.NotCompilable, Files.Mixin);
            var childClass = CreateClass("SimpleChildClassWithInterface");
            var baseInterface = childClass.SourceCode.BaseList.Types[0];

            var command = new CreateMixinFromInterfaceCommand((SimpleBaseTypeSyntax)baseInterface, Semantic);

            var result = command.Execute(childClass.SourceCode, Semantic);

            Assert.IsTrue(result.Members.Count == 2);
            Assert.IsTrue(result.Members.Any(x => x is FieldDeclarationSyntax));
            Assert.IsTrue(result.Members.Any(x => x is MemberDeclarationSyntax));
        }
    }
}