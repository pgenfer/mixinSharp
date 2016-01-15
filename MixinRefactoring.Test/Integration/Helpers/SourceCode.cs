using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MixinRefactoring.Test.TestResourceLoader;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// helper class for interaction with source code.
    /// The class takes several source code files as parameters
    /// and allows access to the syntax and semantic information
    /// of the classes
    /// </summary>
    public class SourceCode
    {
        private readonly SyntaxTree _syntaxTree;
        private readonly SemanticModel _semanticModel;

        public SourceCode(params string[] codeFiles)
        {
            var sourceCodes = codeFiles.Select(x => ReadDummyData(x));
            _syntaxTree = CSharpSyntaxTree.ParseText(string.Concat(sourceCodes));

            var compilation = CSharpCompilation.Create(
                "temp", 
                syntaxTrees: new[] { _syntaxTree },
                // add reference to system assembly (for standard data types)
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
            _semanticModel  = compilation.GetSemanticModel(_syntaxTree);
        }

        public ClassDeclarationSyntax Class(string className) => _syntaxTree.GetRoot().FindClassByName(className);
        public FieldDeclarationSyntax MixinInClass(string className,string mixin) => Class(className).FindMixinReference(mixin);
        public SyntaxTree Syntax => _syntaxTree;
        public SemanticModel Semantic => _semanticModel;   
    }
}
