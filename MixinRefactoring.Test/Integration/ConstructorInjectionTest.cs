using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Linq;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class ConstructorInjectionTest
    {
        private ConstructorDeclarationSyntax InjectMixin<T>()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Constructor, Files.Name);
            var childClass = sourceCode.Class(typeof(T).Name);
            var mixinClass =
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(childClass.FindMixinReference("_name"));
            var strategy = new InjectConstructorImplementationStrategy(
                mixinClass, sourceCode.Semantic, childClass.SpanStart);
            // act
            var newConstructor =
                strategy.ExtendExistingConstructor(
                    childClass
                    .ChildNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .Single());
            return newConstructor;
        }

        private ConstructorInitializerSyntax InjectMixinIntoInitializer<T>()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Constructor, Files.Name);
            var childClass = sourceCode.Class(typeof(T).Name);
            var mixinClass =
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(childClass.FindMixinReference("_name"));
            var strategy = new InjectConstructorImplementationStrategy(
                mixinClass, sourceCode.Semantic, childClass.SpanStart);
            var oldConstructorInitializer = childClass.DescendantNodes()
                .OfType<ConstructorInitializerSyntax>()
                .Single();
            // act
            var newInitializer =
                strategy.ExtendConstructorInitialization(oldConstructorInitializer);
            return newInitializer;
        }

        [Test]
        public void NoConstructorInChild_Generate_ConstructorGenerated()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Constructor, Files.Name);
            var childClass = sourceCode.Class(nameof(ChildClassWithoutConstructor));
            var mixinClass =
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(childClass.FindMixinReference("_name"));
            var strategy = new InjectConstructorImplementationStrategy(
                mixinClass, sourceCode.Semantic, childClass.SpanStart);
            // act
            var newConstructor =
                strategy.CreateNewConstructor(nameof(ChildClassWithoutConstructor));
            // assert: constructor has only one parameter
            Assert.AreEqual(1, newConstructor.ParameterList.Parameters.Count);
            // assert: type and name of constructor parameter
            Assert.NotNull(
                newConstructor.ParameterList.Parameters.Single(x =>
                    x.Identifier.Text == "name" &&
                    x.Type.GetText().ToString() == nameof(Name)));
        }

        [Test(Description ="Do not inject mixins into static constructors")]
        public void StaticConstructorInChild_Generate_MixinNotInjected()
        {
            var oldConstructor = InjectMixin<ChildClassWithStaticConstructor>();
            // assert: the static constructor should not have any parameters    
            Assert.IsEmpty(oldConstructor.ParameterList.Parameters);
        }

        [Test]
        public void DefaultConstructorInChild_Generate_MixinInjected()
        {
            var newConstructor = InjectMixin<ChildClassWithConstructor>();
            // assert: constructor has only one parameter
            Assert.AreEqual(1, newConstructor.ParameterList.Parameters.Count);
            // assert: type and name of constructor parameter
            Assert.NotNull(
                newConstructor.ParameterList.Parameters.Single(x =>
                    x.Identifier.Text == "name" &&
                    x.Type.GetText().ToString() == nameof(Name)));
        }

        [Test]
        public void ConstructorWithParameterInChild_Generate_MixinInjected()
        {
            var newConstructor = InjectMixin<ChildClassWithConstructorParameter>();
            // assert: constructor has two parameters now
            Assert.AreEqual(2, newConstructor.ParameterList.Parameters.Count);
            // assert: type and name of constructor parameter
            Assert.IsTrue(
                newConstructor.ParameterList.Parameters.Any(x =>
                    x.Identifier.Text == "name" &&
                    x.Type.GetText().ToString() == nameof(Name)));
        }

        [Test]
        public void ConstructorWithThisInitializer_Generate_MixinInjectedInInitializer()
        {
            var newInitializer = InjectMixinIntoInitializer<ChildClassWithContructorInitializer>();
            // assert: initializer has only one parameter
            Assert.AreEqual(1, newInitializer.ArgumentList.Arguments.Count);
            // assert: type and name of constructor parameter
            Assert.NotNull(newInitializer.ArgumentList.Arguments.Single(x =>
                    x.GetText().ToString() == "name"));
        }

        [Test(Description ="Initializer already has the mixin parameter, so don't change the initilaizer")]
        public void ConstructorWithThisMixinInitializer_Generate_MixinNotInjectedInInitializer()
        {
            var newInitializer = InjectMixinIntoInitializer<ChildWithMixinConstructorInitializer>();
            // assert: initializer has only one parameter
            Assert.AreEqual(1, newInitializer.ArgumentList.Arguments.Count);
            // assert: type and name of constructor parameter
            Assert.NotNull(newInitializer.ArgumentList.Arguments.Single(x =>
                    x.GetText().ToString() == "name"));
        }

        [Test]
        public void ConsturctorWithMixinExists_Generated_ConstructorNotGeneratedTwice()
        {
            var newConstructor = InjectMixin<ChildClassWithInjectedMixin>();
            // assert: constructor has two parameters now
            Assert.AreEqual(1, newConstructor.ParameterList.Parameters.Count);
            // assert: type and name of constructor parameter
            Assert.IsTrue(
                newConstructor.ParameterList.Parameters.Any(x =>
                    x.Identifier.Text == "name" &&
                    x.Type.GetText().ToString().Trim() == nameof(Name)));
        }


        [Test(Description = "Use explicit naming in initializer because of default argument in initializer")]
        public void ConstructorWithDefaultArgumentInInitializer_Generate_UseExplicitNaming()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Constructor, Files.Name,Files.Worker);
            var childClass = sourceCode.Class(nameof(ChildWitDefaultParameters));
            var mixinClass = 
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(childClass.FindMixinReference("_worker"));
            var strategy = 
                new InjectConstructorImplementationStrategy(
                    mixinClass, sourceCode.Semantic, childClass.SpanStart);
            // get the initializer that does not have any parameters
            var oldConstructorInitializer = childClass.DescendantNodes()
                .OfType<ConstructorInitializerSyntax>()
                .Single(x => x.ArgumentList.Arguments.Count == 0);
            // act
            var newInitializer = strategy.ExtendConstructorInitialization(oldConstructorInitializer);

            // assert: initializer should have explicit naming now
            Assert.AreEqual(1, newInitializer.ArgumentList.Arguments.Count);
            Assert.AreEqual("worker",newInitializer.ArgumentList.Arguments[0].NameColon.Name.GetText().ToString());
        }

        [Test(
            Description = "Do not use explicit naming in initializer because previous default parameter is already set")]
        public void ConstructorWithArgumentInInitializer_Generate_DontUseExplicitNaming()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Constructor, Files.Name, Files.Worker);
            var childClass = sourceCode.Class(nameof(ChildWitDefaultParameters));
            var mixinClass =
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(childClass.FindMixinReference("_worker"));
            var strategy =
                new InjectConstructorImplementationStrategy(
                    mixinClass, sourceCode.Semantic, childClass.SpanStart);
            // get the initializer that does not have any parameters
            var oldConstructorInitializer = childClass.DescendantNodes()
                .OfType<ConstructorInitializerSyntax>()
                .Single(x => x.ArgumentList.Arguments.Count == 1);
            // act
            var newInitializer = strategy.ExtendConstructorInitialization(oldConstructorInitializer);

            // assert: initializer should have two parameters
            Assert.AreEqual(2, newInitializer.ArgumentList.Arguments.Count);
            Assert.IsNull(newInitializer.ArgumentList.Arguments[1].NameColon);
        }

        [Test(
           Description = "There is already an explicit named argument, so use a second explicit name in initializer")]
        public void ConstructorWithExplicitNamedArgumentInInitializer_Generate_UseExplicitNamingAgain()
        {
            // arrange
            var sourceCode = new SourceCode(Files.Constructor, Files.Name, Files.Worker);
            var childClass = sourceCode.Class(nameof(ChildWitDefaultParameters));
            var mixinClass =
                new MixinReferenceFactory(sourceCode.Semantic)
                .Create(childClass.FindMixinReference("_name"));
            var strategy =
                new InjectConstructorImplementationStrategy(
                    mixinClass, sourceCode.Semantic, childClass.SpanStart);
            // get the only initializer that uses a named argument
            var oldConstructorInitializer = childClass.DescendantNodes()
                .OfType<ConstructorInitializerSyntax>()
                .Single(x => x.ArgumentList.Arguments.Any(y => y.NameColon != null));
            // act
            var newInitializer = strategy.ExtendConstructorInitialization(oldConstructorInitializer);

            // assert: initializer should have two parameters, both with explicit naming
            Assert.AreEqual(2, newInitializer.ArgumentList.Arguments.Count);
            Assert.AreEqual(2,newInitializer.ArgumentList.Arguments.Count(x => x.NameColon != null));
        }
    }
}
