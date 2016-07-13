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
    public class MixinReferenceFactoryTest : IntegrationTestBase
    {
        [Test]
        public void CreateMixinFromFieldDefinition_Success()
        {
            WithSourceFiles(Files.Person, Files.Name);
            var personClass = CreateClass(nameof(Person));
            var mixinField = personClass.SourceCode.FindMixinReference("_name");

            var mixinFactory = new MixinReferenceFactory(Semantic);
            var mixin = mixinFactory.Create(mixinField);

            Assert.AreEqual("_name", mixin.Name);
            Assert.AreEqual(3, mixin.Class.Properties.Count());
        }

        [Test]
        public void CreateMixinFromFieldDefinition_Error()
        {
            WithSourceFiles(Files.Person);
            var personClass = CreateClass(nameof(Person));
            var mixinField = personClass.SourceCode.FindMixinReference("_name");

            var mixinFactory = new MixinReferenceFactory(Semantic);
            var mixin = mixinFactory.Create(mixinField);
            // Assert: type could not be resolved and null should be returned instead
            Assert.IsNull(mixin);
        }


        [Test]
        public void CreateMixinFromBaseList_Success()
        {
            WithSourceFiles(Files.NotCompilable, Files.Mixin);
            var personClass = CreateClass("SimpleChildClassWithInterface");
            var baseClass = (SimpleBaseTypeSyntax)personClass.SourceCode.BaseList.Types[0];

            var mixinFactory = new MixinReferenceFactory(Semantic);
            var mixin = mixinFactory.Create(baseClass);

            Assert.IsNotNull(mixin);
        }
    }
}