using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class AddInterfacesToChildSyntaxWriterTest
    {
        private ClassDeclarationSyntax ImplementInterfaceByChild(string childClassName)
        {
            var sourceCode = new SourceCode(Files.Interface);
            var childClass = sourceCode.Class(childClassName);
            var startPos = childClass.GetLocation().SourceSpan.Start;

            // create a mixin class that implements an interface
            var mixin = Substitute.For<MixinReference>();
            var @class = Substitute.For<ClassWithTypeSymbol>();
            @class.IsInterface.Returns(false);
            @class.Interfaces.Returns(
                new InterfaceList(
                    new Interface(sourceCode.GetTypeByName(nameof(IFirstInterface)))));

            mixin.Class.Returns(@class);

            var interfaces = new InterfaceList(new[] 
            {
                new Interface(sourceCode.GetTypeByName(nameof(IFirstInterface)))
            });
            var addInterfaceWriter = new AddInterfacesToChildSyntaxWriter(mixin,sourceCode.Semantic, startPos);
            var newChildClass = addInterfaceWriter.Visit(childClass);

            return (ClassDeclarationSyntax)newChildClass;
        }

        [Test]
        public void ChildWithoutInterface_IncludeMixin_InterfaceAdded()
        {
            var newChildClass = ImplementInterfaceByChild(nameof(ChildWithoutInterface));

            Assert.AreEqual(1, newChildClass.BaseList.Types.Count());
            Assert.IsNotNull(newChildClass.BaseList.Types
                .Single(x => x.TypeName() == nameof(IFirstInterface)));
        }

        [Test]
        public void ChildWithInterface_IncludeMixin_NothingAdded()
        {
            var newChildClass = ImplementInterfaceByChild(nameof(ChildWithInterface));
            // assert that there is still only one item in base list
            Assert.AreEqual(1, newChildClass.BaseList.Types.Count());
            Assert.IsNotNull(newChildClass.BaseList.Types
                .Single(x => x.TypeName() == nameof(IFirstInterface)));
        }

        [Test]
        public void ChildWithOneInterface_IncludeMixinWithManyInterfaces_OnlyMissingInterfacesAdded()
        {
            var sourceCode = new SourceCode(Files.Interface);
            var childClass = sourceCode.Class(nameof(ChildWithInterface));
            var startPos = childClass.GetLocation().SourceSpan.Start;
            var interfaces = new InterfaceList(new[] 
            {
                new Interface(sourceCode.GetTypeByName(nameof(IFirstInterface))),
                new Interface(sourceCode.GetTypeByName(nameof(ISecondInterface))) 
            });

            var @class = Substitute.For<ClassWithTypeSymbol>();
            @class.IsInterface.Returns(false);
            @class.Interfaces.Returns(interfaces);
            var mixin = Substitute.For<MixinReference>();
            mixin.Class.Returns(@class);

            var addInterfaceWriter = new AddInterfacesToChildSyntaxWriter(mixin, sourceCode.Semantic, startPos);
            var newChildClass = (ClassDeclarationSyntax)addInterfaceWriter.Visit(childClass);

            var baseList = newChildClass.BaseList.Types;
            // assert: child class should have both interfaces
            Assert.AreEqual(2, baseList.Count);
            Assert.IsNotNull(
                baseList.Single(x => x.TypeName() == nameof(IFirstInterface)));
            Assert.IsNotNull(
                baseList.Single(x => x.TypeName() == nameof(ISecondInterface)));
        }

        [Test]
        public void MixinIsInterface_Include_MixinAddedAsInterface()
        {
            var sourceCode = new SourceCode(Files.Interface);
            var childClass = sourceCode.Class(nameof(ChildWithoutInterface));
            var startPos = childClass.GetLocation().SourceSpan.Start;
            // the mixin will be an interface
            var @class = Substitute.For<ClassWithTypeSymbol>();
            @class.IsInterface.Returns(true);
            @class.AsInterface().Returns(new Interface(sourceCode.GetTypeByName(nameof(IFirstInterface))));
            var mixin = Substitute.For<MixinReference>();
            mixin.Class.Returns(@class);
            // act
            var addInterfaceWriter = new AddInterfacesToChildSyntaxWriter(mixin, sourceCode.Semantic, startPos);
            var newChildClass = (ClassDeclarationSyntax)addInterfaceWriter.Visit(childClass);
            var baseList = newChildClass.BaseList.Types;
            // assert: child class should have the mixin as interface
            Assert.AreEqual(1, baseList.Count);
            Assert.IsNotNull(baseList.Single(x => x.TypeName() == nameof(IFirstInterface)));
        }

        [Test]
        public void MixinIsInterfaceInDifferentNamespace_Include_MixinAddedAsFullQualifiedInterface()
        {
            var sourceCode = new SourceCode(Files.Interface);
            var childClass = sourceCode.Class(nameof(ChildWithoutInterface));
            // changing the start position here will force the type resolver
            // to use a full qualified type name
            var startPos = 0;
            // the mixin will be an interface
            var @class = Substitute.For<ClassWithTypeSymbol>();
            @class.IsInterface.Returns(true);
            @class.AsInterface().Returns(new Interface(
                sourceCode.GetTypeByName(nameof(IFirstInterface))));
            var mixin = Substitute.For<MixinReference>();
            mixin.Class.Returns(@class);

            // act
            var addInterfaceWriter = new AddInterfacesToChildSyntaxWriter(mixin, sourceCode.Semantic, startPos);
            var newChildClass = (ClassDeclarationSyntax)addInterfaceWriter.Visit(childClass);
            var baseList = newChildClass.BaseList.Types;
            // assert: child class should have the mixin as interface
            Assert.AreEqual(1, baseList.Count);
            Assert.IsNotNull(baseList.Single(x => x.TypeName() == "MixinRefactoring.Test.IFirstInterface"));
        }
    }
}
