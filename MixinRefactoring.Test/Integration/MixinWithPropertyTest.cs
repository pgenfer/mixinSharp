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
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("Person");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            // 2. create instances for mixin and child mixin
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);
            
            // act 
            child.Include(mixin);

            // assert. The child should have the same properties as the mixin
            Assert.AreEqual(child.Members.Count(), mixin.Services.Count());
            // check that every method of mixin appears only once in the child
            foreach (var service in mixin.Services)
                Assert.AreEqual(1,child.Members.Count(x => x.Name == service.Name));
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
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert. The child should have the same properties as the mixin
            Assert.AreEqual(child.Members.Count(), mixin.Services.Count());
            // check that every method of mixin appears only once in the child
            foreach (var service in mixin.Services)
                Assert.AreEqual(1, child.Members.Count(x => x.Name == service.Name));
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
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert. Only one property in resulting class
            Assert.AreEqual(1,child.Members.Count());
            // that one property should only have a getter
            Assert.AreEqual(1, child.Members.Count(
                x => ((PropertyServiceBase)x).HasGetter &&
                     !((PropertyServiceBase)x).HasSetter));
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
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert. Only one property in resulting class
            Assert.AreEqual(1, child.Members.Count());
            // the expression body should be converted to a HasGetter
            Assert.AreEqual(1, child.Members.Count(
                x => ((PropertyServiceBase)x).HasGetter &&
                     !((PropertyServiceBase)x).HasSetter));
        }

        [Test]
        public void ChildHasPropertyAlready_Include_OnlyOnePropertyInChild()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("PersonWithName");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert: Child should only have one "Name" property
            Assert.AreEqual(1, child.Members.Count(x => x.Name == "Name"));
            // this one property should have getter and setter
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasGetter);
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasSetter);
        }

        [Test]
        public void ChildHasBaseClassWithProperty_Include_PropertyNotReimplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("DerivedPersonClass");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert: Child should only have one "Name" property
            Assert.AreEqual(1, child.Members.Count(x => x.Name == "Name"));
            // this one property should have getter and setter
            // because it used the base implementation, not the version from the mixin
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasGetter);
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasSetter);
        }

        [Test]
        public void ChildHasBaseClassWithBaseClassesWithProperty_Include_PropertyNotReimplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("ThirdPersonClass");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert: Child should only have one "Name" property
            Assert.AreEqual(1, child.Members.Count(x => x.Name == "Name"));
            // this one property should have getter and setter
            // because it used the base implementation, not the version from the mixin
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasGetter);
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasSetter);
        }

        [Test]
        public void ChildHasInterfaceWithProperty_Include_PropertyImplemented()
        {
            // arrange
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClass = sourceCode.Class("DerivedFromInterfaceClass");
            var mixinReference = personClass.FindMixinReference("_name");
            var semanticModel = sourceCode.Semantic;
            var mixin = new MixinFactory(semanticModel).FromFieldDeclaration(mixinReference);
            var child = new MixinChild(personClass, semanticModel);

            // act 
            child.Include(mixin);

            // assert: Child should have one "Name" property
            Assert.AreEqual(1, child.Members.Count(x => x.Name == "Name"));
            // this one property should have only getter (signature from interface and mixin is the same)
            Assert.IsTrue(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasGetter);
            Assert.IsFalse(((PropertyServiceBase)child.Members.Single(x => x.Name == "Name")).HasSetter);
        }

        

        // TODO: check what to do with abstract properties
        // TODO: ignore static properties
    }
}
