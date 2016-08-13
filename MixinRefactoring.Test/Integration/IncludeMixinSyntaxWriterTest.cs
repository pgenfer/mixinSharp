using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using NSubstitute;

namespace MixinRefactoring.Test
{
    public class IncludeMixinSyntaxWriterTest : IntegrationTestBase
    {
        /// <summary>
        /// syntax writer dummy where strategies can be injected
        /// </summary>
        public class IncludeSyntaxWriterTestDummy : IncludeMixinSyntaxWriter
        {
            /// <summary>
            /// stores a reference to a strategy that will be injected during test case
            /// </summary>
            private readonly IImplementMemberForwarding _strategyThatSouldBeCalled;
            /// <summary>
            /// the type of the member for which the strategy should be used
            /// </summary>
            private readonly Type _typeForWhichStrategyIsUsed;

            protected override Dictionary<Type, IImplementMemberForwarding> CreateStrategies(
                MixinReference mixin, 
                SemanticModel semantic, 
                Settings settings)
            {
                return new Dictionary<Type, IImplementMemberForwarding>
                {
                    [_typeForWhichStrategyIsUsed] = _strategyThatSouldBeCalled
                };
            }

            public IncludeSyntaxWriterTestDummy(
                IEnumerable<Member> members,
                MixinReference mixin, 
                Type typeForStrategy,
                IImplementMemberForwarding strategy)
                :base(members, mixin, null)
            {
                _typeForWhichStrategyIsUsed = typeForStrategy;
                _strategyThatSouldBeCalled = strategy;
            }            
        }

        [TestDescription("Check that the strategy for generating properties is called")]
        public void PropertiesToImplement_WriteSyntax_PropertyStrategyCalled()
        {
            WithSourceFiles(Files.Person, Files.Name);
            var person = CreateClass(nameof(Person));
            var mixin = CreateMixinReference(person, "_name");

            var propertyStrategy = Substitute.For<IImplementMemberForwarding>();
             
            var includeWriter = new IncludeSyntaxWriterTestDummy(
                mixin.Class.Properties,
                mixin, 
                typeof(Property),
                propertyStrategy);
            includeWriter.Visit(person.SourceCode);

            // ensure that the implementMember of the propertyStrategy was called
            propertyStrategy.Received().ImplementMember(Arg.Any<Member>(), Arg.Any<int>());          
        }

        [TestDescription("Check that strategy for generating events is called")]
        public void EventsToImplement_WriteSyntax_EventStrategyCalled()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);
            var childClass = CreateClass(nameof(ChildClassWithEventsFromMixin));
            var mixin = CreateMixinReference(childClass, "_mixin");

            var eventStrategy = Substitute.For<IImplementMemberForwarding>();

            var includeWriter = new IncludeSyntaxWriterTestDummy(
                mixin.Class.Events,
                mixin,
                typeof(Event),
                eventStrategy);
            includeWriter.Visit(childClass.SourceCode);

            eventStrategy.Received().ImplementMember(mixin.Class.Events.Single(), Arg.Any<int>());

        }

        [Test]
        public void OverrideMethodToImplement_WriteSyntax_MethodHasOverrideModifier()
        {
            WithSourceFiles(Files.Person, Files.Worker);
            var person = CreateClass(nameof(PersonWithToString));
            var worker = CreateMixinReference(person, "_toString");
            
            var includeWriter = new IncludeMixinSyntaxWriter(worker.Class.Methods, worker, Semantic);
            var newPersonClassSource = includeWriter.Visit(person.SourceCode);
            // check that new person class has a method that is overriden
            var methodDeclaration = newPersonClassSource.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single()
                .NormalizeWhitespace()
                .GetText()
                .ToString();
            Assert.IsTrue(methodDeclaration.Contains("override"));
        }

        [Test]
        public void MixinWithIndexer_WriteSyntax_IndexerImplementedInChild()
        {
            WithSourceFiles(Files.Person, Files.Collection);
            var person = CreateClass(nameof(PersonWithIndexer));
            var collection = CreateMixinReference(person, "_collection");

            var includeWriter = new IncludeMixinSyntaxWriter(collection.Class.Properties, collection, Semantic);
            var newPersonClassSource = includeWriter.Visit(person.SourceCode);
            // check that generated class code has exactly one indexer declaration
            var indexerDeclaration = newPersonClassSource.DescendantNodes()
                .OfType<IndexerDeclarationSyntax>().SingleOrDefault();
            Assert.IsNotNull(indexerDeclaration);
        }

        [Test]
        public void MethodWithOverride_WriteSyntax_OverrideImplementedInChild()
        {
            WithSourceFiles(Files.NotCompilable, Files.Worker);
            var person = CreateClass("PersonFromAbstractWork");
            var worker = CreateMixinReference(person, "_worker");
            // create a worker method with an override keyword
            var methods = worker.Class.Methods.Select(x => x.Clone(true));

            var includeWriter = new IncludeMixinSyntaxWriter(methods, worker, Semantic);
            var newPersonClassSource = includeWriter.Visit(person.SourceCode);
            // check that generated class code has exactly one indexer declaration
            var methodDeclaration = newPersonClassSource.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single()
                .NormalizeWhitespace()
                .GetText()
                .ToString();
            Assert.IsTrue(methodDeclaration.Contains("override"));
        }

        [Test]
        public void PropertyWithOverride_WriteSyntax_OverrideImplementedInChild()
        {
            WithSourceFiles(Files.NotCompilable, Files.Name);
            var person = CreateClass("PersonFromAbstractName");
            var name = CreateMixinReference(person, "_name");
            // create a worker method with an override keyword
            var properties = name.Class.Properties.Select(x => x.Clone(true));

            var includeWriter = new IncludeMixinSyntaxWriter(properties, name, Semantic);
            var newPersonClassSource = includeWriter.Visit(person.SourceCode);
            // check that generated class code has exactly one indexer declaration
            var propertyDeclaration = newPersonClassSource.DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Single()
                .NormalizeWhitespace()
                .GetText()
                .ToString();
            Assert.IsTrue(propertyDeclaration.Contains("override"));
        }
    }
}
