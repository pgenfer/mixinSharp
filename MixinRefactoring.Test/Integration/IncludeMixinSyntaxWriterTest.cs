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
                IEnumerable<Member> members,string name,IImplementMemberForwarding propertyStrategy)
                :base(members, name,null)
            {
                _propertyStrategy = propertyStrategy;
            }            
        }

        [Test]
        public void PropertiesToImplement_WriteSyntax_PropertyStrategyCalled()
        {
            var sourceCode = new SourceCode("Person.cs","Name.cs");
            var personClassSource = sourceCode.Class("Person");
            var nameClassSource = sourceCode.Class("Name");
      
            var nameClass = new ClassFactory(sourceCode.Semantic).Create(nameClassSource);

            var propertyStrategy = Substitute.For<IImplementMemberForwarding>();
            
            var includeWriter = new IncludeSyntaxWriterTestDummy(nameClass.Properties, "_name",propertyStrategy);
            var newPersonClassSource = includeWriter.Visit(personClassSource);

            // ensure that the implementMember of the propertyStrategy was called
            propertyStrategy.Received().ImplementMember(Arg.Any<Member>(), Arg.Any<int>());          
        }

        [Test]
        public void OverrideMethodToImplement_WriteSyntax_MethodHasOverrideModifier()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClassSource = sourceCode.Class("PersonWithToString");
            var worker = new MixinReferenceFactory(sourceCode.Semantic).Create(personClassSource.FindMixinReference("_toString"));
            
            var includeWriter = new IncludeMixinSyntaxWriter(worker.Class.Methods, "_toString",sourceCode.Semantic);
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
            var sourceCode = new SourceCode("Person.cs", "Collection.cs");
            var personClassSource = sourceCode.Class("PersonWithIndexer");
            var collection = new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClassSource.FindMixinReference("_collection"));

            var includeWriter = new IncludeMixinSyntaxWriter(collection.Class.Properties, "_collection", sourceCode.Semantic);
            var newPersonClassSource = includeWriter.Visit(personClassSource);
            // check that generated class code has exactly one indexer declaration
            var indexerDeclaration = newPersonClassSource.DescendantNodes()
                .OfType<IndexerDeclarationSyntax>().SingleOrDefault();
            Assert.IsNotNull(indexerDeclaration);
        }

        [Test]
        public void MethodWithOverride_WriteSyntax_OverrideImplementedInChild()
        {
            var sourceCode = new SourceCode("Person.cs", "Worker.cs");
            var personClassSource = sourceCode.Class("PersonFromAbstractWork");
            var worker = new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClassSource.FindMixinReference("_worker"));
            // create a worker method with an override keyword
            var methods = worker.Class.Methods.Select(x => x.Clone(true));

            var includeWriter = new IncludeMixinSyntaxWriter(methods, "_worker", sourceCode.Semantic);
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
            var sourceCode = new SourceCode("Person.cs", "Name.cs");
            var personClassSource = sourceCode.Class("PersonFromAbstractName");
            var name = new MixinReferenceFactory(sourceCode.Semantic)
                .Create(personClassSource.FindMixinReference("_name"));
            // create a worker method with an override keyword
            var properties = name.Class.Properties.Select(x => x.Clone(true));

            var includeWriter = new IncludeMixinSyntaxWriter(properties, "_name", sourceCode.Semantic);
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
