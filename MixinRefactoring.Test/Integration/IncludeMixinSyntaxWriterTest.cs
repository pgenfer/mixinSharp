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
    [TestFixture]
    public class IncludeMixinSyntaxWriterTest
    {
        /// <summary>
        /// syntax writer dummy where strategies can be injected
        /// </summary>
        public class IncludeSyntaxWriterTestDummy : IncludeMixinSyntaxWriter
        {
            /// <summary>
            /// stores a reference to the property strategy that will be injected during test case
            /// </summary>
            private readonly IImplementMemberForwarding _propertyStrategy;

            protected override Dictionary<Type, IImplementMemberForwarding> CreateStrategies(string name, SemanticModel semantic, Settings settings)
            {
                return new Dictionary<Type, IImplementMemberForwarding>()
                {
                    [typeof(Property)] = _propertyStrategy
                };
            }

            public IncludeSyntaxWriterTestDummy(
                IEnumerable<Member> members,MixinReference mixin, IImplementMemberForwarding propertyStrategy)
                :base(members, mixin, null)
            {
                _propertyStrategy = propertyStrategy;
            }            
        }

        [Test]
        public void PropertiesToImplement_WriteSyntax_PropertyStrategyCalled()
        {
            var sourceCode = new SourceCode(Files.Person,Files.Name);
            var personClassSource = sourceCode.Class(nameof(Person));
            var nameClassSource = sourceCode.Class(nameof(Name));
      
            var nameClass = new ClassFactory(sourceCode.Semantic).Create(nameClassSource);

            var propertyStrategy = Substitute.For<IImplementMemberForwarding>();
            // only a test dummy
            var mixin = new MixinReference("_name", null);            
            
            var includeWriter = new IncludeSyntaxWriterTestDummy(nameClass.Properties,mixin, propertyStrategy);
            var newPersonClassSource = includeWriter.Visit(personClassSource);

            // ensure that the implementMember of the propertyStrategy was called
            propertyStrategy.Received().ImplementMember(Arg.Any<Member>(), Arg.Any<int>());          
        }

        [Test]
        public void OverrideMethodToImplement_WriteSyntax_MethodHasOverrideModifier()
        {
            var sourceCode = new SourceCode(Files.Person, Files.Worker);
            var personClassSource = sourceCode.Class(nameof(PersonWithToString));
            var worker = new MixinReferenceFactory(sourceCode.Semantic).Create(personClassSource.FindMixinReference("_toString"));
            
            var includeWriter = new IncludeMixinSyntaxWriter(worker.Class.Methods, worker, sourceCode.Semantic);
            var newPersonClassSource = includeWriter.Visit(personClassSource);
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
            var sourceCode = new SourceCode(Files.Person, Files.Collection);
            var personClassSource = sourceCode.Class(nameof(PersonWithIndexer));
            var collection = new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClassSource.FindMixinReference("_collection"));

            var includeWriter = new IncludeMixinSyntaxWriter(collection.Class.Properties, collection, sourceCode.Semantic);
            var newPersonClassSource = includeWriter.Visit(personClassSource);
            // check that generated class code has exactly one indexer declaration
            var indexerDeclaration = newPersonClassSource.DescendantNodes()
                .OfType<IndexerDeclarationSyntax>().SingleOrDefault();
            Assert.IsNotNull(indexerDeclaration);
        }

        [Test]
        public void MethodWithOverride_WriteSyntax_OverrideImplementedInChild()
        {
            var sourceCode = new SourceCode(Files.NotCompilable, Files.Worker);
            var personClassSource = sourceCode.Class("PersonFromAbstractWork");
            var worker = new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClassSource.FindMixinReference("_worker"));
            // create a worker method with an override keyword
            var methods = worker.Class.Methods.Select(x => x.Clone(true));

            var includeWriter = new IncludeMixinSyntaxWriter(methods, worker, sourceCode.Semantic);
            var newPersonClassSource = includeWriter.Visit(personClassSource);
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
            var sourceCode = new SourceCode(Files.NotCompilable, Files.Name);
            var personClassSource = sourceCode.Class("PersonFromAbstractName");
            var name = new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClassSource.FindMixinReference("_name"));
            // create a worker method with an override keyword
            var properties = name.Class.Properties.Select(x => x.Clone(true));

            var includeWriter = new IncludeMixinSyntaxWriter(properties, name, sourceCode.Semantic);
            var newPersonClassSource = includeWriter.Visit(personClassSource);
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
