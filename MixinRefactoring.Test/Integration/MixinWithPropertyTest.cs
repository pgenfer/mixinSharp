﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Linq;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class MixinWithPropertyTest
    {
        [Test]
        public void MixinClassWithProperty_Include_PropertiesIncluded()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("Person");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // assert all properties of the mixin should have been added
            Assert.AreEqual(mixin.Class.Properties.Count(),mixer.PropertiesToImplement.Count());
            // check that every method of mixin appears only once in the child
            foreach (var service in mixin.Class.Properties)
                Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.Name == service.Name));
        }

        [Test]
        public void MixinInterfaceWithProperty_Include_PropertiesIncluded()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("Person");
            var mixinReference = personClass.FindMixinReference("_interfaceName");
            var semanticModel = sourceCode.Semantic;
            // 2. create instances for mixin and child mixin
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // assert. The child should have the same properties as the mixin
            Assert.AreEqual(mixin.Class.Properties.Count(), mixer.PropertiesToImplement.Count());
            // check that every method of mixin appears only once in the child
            foreach (var service in mixin.Class.Properties)
                Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.Name == service.Name));
        }

        [Test]
        public void MixinWithPropertyOnlyGetter_Include_PropertyHasGetter()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithGetterName");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            // 2. create instances for mixin and child mixin
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // assert. Only one property in resulting class
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count());
            // that one property should only have a getter
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.IsReadOnly));
        }

        [Test]
        public void MixinWithPropertyExpressionBody_Include_PropertyHasGetter()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithGetterName");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            // 2. create instances for mixin and child mixin
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // assert. Only one property in resulting class
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count());
            // the expression body should be converted to a HasGetter
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.IsReadOnly));
        }

        [Test]
        public void ChildHasPropertyAlready_Include_OnlyOnePropertyInChild()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithName");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // nothing to implement for the mixer, child already has property
            Assert.IsEmpty(mixer.PropertiesToImplement);
        }

        [Test]
        public void ChildHasBaseClassWithProperty_Include_PropertyNotReimplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("DerivedPersonClass");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // nothing to implement for the mixer, child already has property
            Assert.IsEmpty(mixer.PropertiesToImplement);
        }

        [Test]
        public void ChildHasBaseClassWithBaseClassesWithProperty_Include_PropertyNotReimplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("ThirdPersonClass");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // nothing to implement for the mixer, child already has property
            Assert.IsEmpty(mixer.PropertiesToImplement);
        }

        [Test]
        public void ChildHasInterfaceWithProperty_Include_PropertyImplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("DerivedFromInterfaceClass");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // assert: Child should have one "Name" property
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.Name == "Name"));
            // this one property should have only getter (signature from interface and mixin is the same)
            Assert.IsTrue(mixer.PropertiesToImplement.Single(x => x.Name == "Name").IsReadOnly);
        }

        [Test]
        public void MixinWithStaticProperty_Include_PropertyNotImplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithStaticMixin");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // assert: Child should not have a "Name" property
            Assert.IsEmpty(mixer.PropertiesToImplement);
        }

        [Test]
        public void MixinWithGenericProperty_Include_PropertyImplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithGenericMixin");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // child should have "Name" property
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.Name == "Names"));
            // name property should be of type "IEnumerable<string>"
            var typeName = mixer.PropertiesToImplement.Single().Type.ToString();
            Assert.AreEqual("System.Collections.Generic.IEnumerable<string>", typeName);
        }

        [Test]
        public void MixinWithIndexer_Include_IndexerImplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Collection.cs");
            var personClass = sourceCode.Class("PersonWithIndexer");
            var mixinReference = personClass.FindMixinReference("_collection");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            // act 
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // child should also have an indexer property now
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count());
            Assert.AreEqual("string this[int index]", mixer.PropertiesToImplement.Single().ToString());            
        }

        [Test]
        public void ChildWithAbstractProperty_Include_AbstractPropertyOverridden()
        {
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonFromAbstractName");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // there should be two methods, from base mixin and derived mixin
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count());
            // only one method from the mixin should be implemented, the other one
            // is alredy implemented by childs base
            Assert.AreEqual(1, mixer.PropertiesToImplement.Count(x => x.Name == "Name"));
            Assert.IsTrue(mixer.PropertiesToImplement.Single().IsOverride);
        }

        /// <summary>
        /// check that issue
        /// https://github.com/pgenfer/mixinSharp/issues/6
        /// is also fixed for properties
        /// </summary>
        [Test]
        public void ChildWithOverrideProperty_Include_PropertyOverrideNotCreated()
        {
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithOverriddenProperty");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;

            var mixin = new MixinReferenceFactory(semanticModel).Create(mixinReference);
            var child = new ClassFactory(semanticModel).Create(personClass);

            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, child);

            // no property to override because child overrides it already
            Assert.AreEqual(0, mixer.PropertiesToImplement.Count());
        }
    }
}
