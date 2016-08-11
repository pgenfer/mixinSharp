﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring.Test
{
    public class MixinWithMethodTest : IntegrationTestBase
    {
        [Test]
        public void MixinWithMethod_Include_MethodsIncluded()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(Person));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            Assert.AreEqual(mixer.MethodsToImplement.Count(), mixin.Class.Methods.Count());
            foreach (var service in mixin.Class.Methods)
                Assert.AreEqual(1, mixer.MethodsToImplement.Count(x => x.Name == service.Name));
        }

        [Test]
        public void MethodAlreadyImplemented_Include_MethodNotIncluded()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithWorkMethod));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no method to implement
            Assert.IsEmpty(mixer.MethodsToImplement);
        }

        [Test]
        public void MethodImplementedWithOtherParameter_Include_MethodIncluded()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithOtherWorkMethod));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no method to implement
            Assert.AreEqual(1,mixer.MethodsToImplement.Count(x => x.Name == "Work"));
        }

        [Test]
        public void MixinWithStaticMethod_Include_MethodNotIncluded()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithStaticMethodMixin));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no method to implement
            Assert.IsEmpty(mixer.MethodsToImplement);
        }

        [Test]
        public void MixinWithToString_Include_ToStringShouldBeImplemented()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithToString));
            var mixinReference = personClass.FindMixinReference("_toString");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // ToString should be in list of methods to override
            Assert.IsTrue(mixer.MethodsToImplement.Any(x => x.Name == "ToString"));
            // ToString in mixin must have override keyword
            Assert.IsTrue(mixer.MethodsToImplement.Single(x => x.Name == "ToString").IsOverrideFromObject);
        }

        [Test]
        public void MixinWithGenericParameter_Include_MixinImplemented()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithGenericClassMixin));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should be one method to implement
            Assert.AreEqual(1, mixer.MethodsToImplement.Count());
            // parameter and return type of the method should be int
            Assert.AreEqual("int", mixer.MethodsToImplement.Single().ReturnType.ToString());
            Assert.AreEqual("int", mixer.MethodsToImplement.Single().GetParameter(0).Type.ToString());
        }

        [Test]
        public void MixinWithBaseClass_Include_BothMethodsImplemented()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithDerivedWorker));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should be two methods, from base mixin and derived mixin
            Assert.AreEqual(2, mixer.MethodsToImplement.Count());
            // method from base should be in the implementation list
            Assert.AreEqual(1, mixer.MethodsToImplement.Count(x => x.Name == "Work"));
            // method from derived should be in the implementation list
            Assert.AreEqual(1, mixer.MethodsToImplement.Count(x => x.Name == "AdditionalWork"));            
        }


        [Test]
        public void ChildWithBaseMethod_Include_BaseMethodNotImplemented()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(DerivedPerson));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should be two methods, from base mixin and derived mixin
            Assert.AreEqual(1, mixer.MethodsToImplement.Count());
            // only one method from the mixin should be implemented, the other one
            // is alredy implemented by childs base
            Assert.AreEqual(1, mixer.MethodsToImplement.Count(x => x.Name == "Work"));
        }

        [Test(
         Description = "If the abstract method is in the base class, override it when including the mixin")]
        public void ChildWithAbstractMethodFromBase_Include_AbstractMethodOverridden()
        {
            // file Person with base class is also needed here
            var sourceCode = new SourceCode(Files.Person, Files.NotCompilable, Files.Worker);
            var personClass = sourceCode.Class("PersonFromAbstractWork");
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should be one method that overrides the abstract method
            Assert.AreEqual(1, mixer.MethodsToImplement.Count());
            // only one method from the mixin should be implemented, the other one
            // is alredy implemented by childs base
            Assert.AreEqual(1, mixer.MethodsToImplement.Count(x => x.Name == "Work"));
            Assert.IsTrue(mixer.MethodsToImplement.Single().IsOverride);
        }

        [Test(
         Description = "Dont create an override method if the abstract method is declared in the child itself")]
        public void ChildWithAbstractMethod_Include_MethodOverrideNotCreated()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClass = sourceCode.Class(nameof(PersonWithAbstractMethod));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should not be any method to override,
            // because the abstract method is in the child class itself
            Assert.AreEqual(0, mixer.MethodsToImplement.Count());
        }

        /// <summary>
        /// check for issue
        /// https://github.com/pgenfer/mixinSharp/issues/6
        /// A method override should not be added if an override with the
        /// same signature exists already in the child class
        /// </summary>
        [Test]
        public void ChildWithOverrideMethod_Include_MethodOverrideNotCreated()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker); 
            var personClass = sourceCode.Class(nameof(PersonWithOverriddenMethod));
            var mixinReference = personClass.FindMixinReference("_worker");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should not be any method to override,
            // because the child itself already overrides the method
            Assert.AreEqual(0, mixer.MethodsToImplement.Count());
        }

        [TestDescription(
            @"Check that issue https://github.com/pgenfer/mixinSharp/issues/21 is fixed.
              Private and protected methods should be ignored during generation")]
        public void ChildHasHiddenMethods_Include_MethodsNotGenerated()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);

            var child = CreateClass(nameof(ChildClassWithHiddenMethodMixin));
            var mixin = CreateMixinReference(child, "_mixin");

            var includeMixin = new IncludeMixinCommand(mixin);
            var generatedClass = includeMixin.Execute(child.SourceCode, Semantic);

            Assert.IsEmpty(generatedClass.Members.OfType<MethodDeclarationSyntax>());
        }
    }
}
