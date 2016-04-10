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

        // TODO: check default parameter handling:
        // what if there are some default parameters before in the constructor initalizer that have no values?
        // in that case, the argument name should be set explicitly
    }
}
