using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        protected void WithExternalAssemblyFromType(Type externalType)
        {
            _sourceCode = new SourceCode();
            _sourceCode.CompileFromAssemblyOfType(externalType);
        }

        protected ClassWithSourceCode CreateClass(string className) => _sourceCode.CreateClass(className);

        /// <summary>
        /// creates a mixin reference
        /// </summary>
        /// <param name="child">child class where the mixin reference is placed</param>
        /// <param name="mixinField">name of the field in the child class
        /// that references the mixin</param>
        /// <returns></returns>
        protected MixinReference CreateMixinReference(ClassWithSourceCode child, string mixinField) => 
            new MixinReferenceFactory(_sourceCode.Semantic).Create(
                _sourceCode.MixinInClass(child.Name, mixinField));
        /// <summary>
        /// creates a mixin reference
        /// </summary>
        /// <param name="mixinName">name of the mixin reference</param>
        /// <param name="mixinTypeName">name of the type the mixin reference should be</param>
        /// <returns></returns>
        protected MixinReference CreateMixinReference(string mixinName, string mixinTypeName) =>
            new MixinReference(mixinName, new ClassFactory(Semantic)
                .Create(_sourceCode.GetTypeByName(mixinTypeName)));
        /// <summary>
        /// returns the semantic model
        /// </summary>
        protected SemanticModel Semantic => _sourceCode.Semantic;
    }
}
