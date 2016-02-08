using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class IncludeMixinSyntaxWriterTest
    {
        public class IncludeSyntaxWriterTestDummy : IncludeMixinSyntaxWriter
        {
            public bool ImplementPropertyCalled { get; set; }

            public IncludeSyntaxWriterTestDummy(IEnumerable<Member> members,string name):base(members, name,null)
            { }

            protected override MemberDeclarationSyntax ImplementDelegation(Property property)
            {
                ImplementPropertyCalled = true;
                return null;
            }
        }

        [Test]
        public void PropertiesToImplement_WriteSyntax_SyntaxWritten()
        {
            var sourceCode = new SourceCode("Person.cs","Name.cs");
            var personClassSource = sourceCode.Class("Person");
            var nameClassSource = sourceCode.Class("Name");
      
            var nameClass = new ClassFactory(sourceCode.Semantic).Create(nameClassSource);

            var includeWriter = new IncludeSyntaxWriterTestDummy(nameClass.Properties, "_name");
            var newPersonClassSource = includeWriter.Visit(personClassSource);

            Assert.IsTrue(includeWriter.ImplementPropertyCalled);            
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
    }
}
