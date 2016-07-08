using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// base class that contains some common functionality used
    /// in integration tests.
    /// A new integration test class can inherit from this to
    /// access all relevant helper functions
    /// </summary>
    [TestFixture]
    public class IntegrationTestBase
    {
        private SourceCode _sourceCode;

        protected void WithSourceFiles(params string[] sourceFiles)
        {
            _sourceCode = new SourceCode(sourceFiles);
        }

        protected ClassWithSourceCode CreateClass(string className) => _sourceCode.CreateClass(className);
        protected MixinReference CreateMixinReference(ClassWithSourceCode child, string mixinField) => 
            new MixinReferenceFactory(_sourceCode.Semantic).Create(
                _sourceCode.MixinInClass(child.Name, mixinField));
        protected SemanticModel Semantic => _sourceCode.Semantic;
    }
}
