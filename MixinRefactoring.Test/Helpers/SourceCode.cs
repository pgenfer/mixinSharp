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
    /// of the classes.
    /// This class now also has a fluent API to add source code
    /// on the fly. Before using the fluent API, ensure that you call New() to erase
    /// all existing content (if desired) and at the end, call Compile() to generate
    /// a compilation unit and a semantic model from the collected source code.
    /// </summary>
    public class SourceCode
    {
        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;
        private Compilation _compilation;
        private readonly List<string> _fileContent = new List<string>();

        public SourceCode(params string[] codeFiles)
        {
            AddFiles(codeFiles).Compile();
        }

        /// <summary>
        /// empty constructor can be used when loading
        /// external assemblies
        /// </summary>
        public SourceCode() { }

        public void CompileFromAssemblyOfType(Type externalType)
        {
            _syntaxTree = CSharpSyntaxTree.ParseText(string.Empty);
            var externalAssembly = MetadataReference.CreateFromFile(externalType.Assembly.Location);
            _compilation = CSharpCompilation.Create(
                    "temp",
                    syntaxTrees: new[] { _syntaxTree },
                    references: new[] { externalAssembly });
            _semanticModel = _compilation.GetSemanticModel(_syntaxTree);
        }

        private void CreateSemanticModelFromSource(IEnumerable<string> fileContent)
        {
            _syntaxTree = CSharpSyntaxTree.ParseText(string.Concat(fileContent));

            _compilation = CSharpCompilation.Create(
                "temp",
                syntaxTrees: new[] { _syntaxTree },
                // add reference to system assembly (for standard data types)
                references: MetaDataReferenceResolver.ResolveSystemAssemblies());
            _semanticModel = _compilation.GetSemanticModel(_syntaxTree);
        }

        /// <summary>
        /// fluent API, resets the file list that
        /// were used to generate the source code
        /// </summary>
        /// <returns></returns>
        public SourceCode New()
        {
            _fileContent.Clear();
            return this;
        }

        /// <summary>
        /// add new files to the list of files for this source code
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public SourceCode AddFiles(params string[] fileNames)
        {
            _fileContent.AddRange(fileNames.Select(ReadDummyData).ToList());
            return this;
        }

        /// <summary>
        /// adds the content of a file to this source code
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public SourceCode AddSource(string content)
        {
            _fileContent.Add(content);
            return this;
        }

        /// <summary>
        /// creates the semantic model out of all the
        /// source code that was added to this instance
        /// </summary>
        public void Compile()
        {
            CreateSemanticModelFromSource(_fileContent);
        }

        public ClassDeclarationSyntax Class(string className) => _syntaxTree.GetRoot().FindClassByName(className);
        public ClassWithSourceCode CreateClass(string className) => new ClassFactory(Semantic).Create(Class(className));
        public FieldDeclarationSyntax MixinInClass(string className, string mixin) => Class(className).FindMixinReference(mixin);
        public ITypeSymbol GetTypeByName(string className)
        {
            var classDeclaration = Class(className);
            if (classDeclaration == null)
            {
                var typeSymbol = _compilation.GetTypeByMetadataName($"MixinRefactoring.Test.{className}");
                return typeSymbol;
            }
            return Semantic.GetDeclaredSymbol(classDeclaration);
        }
        public SyntaxTree Syntax => _syntaxTree;
        public SemanticModel Semantic => _semanticModel;
    }
}
