using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.IO.Path;

namespace MixinRefactoring.Test
{
    public static class TestResourceLoader
    {
        private static Assembly _assembly;

        static TestResourceLoader()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }        

        public static string ReadDummyData(string fileName)
        {
            // tests are executed in a different directory,
            // so we need the directory where the test assemblies are
            // placed to find the dummy resources
            var testDirectory = TestContext.CurrentContext.TestDirectory;
            var resourceFolder = Combine(testDirectory, @"integration\dummys");
            var file = Combine(resourceFolder, fileName);
            var text = File.ReadAllText(file);
            //string text = string.Empty;
            //var path = "MixinRefactoring.Test.Integration.Dummys";
            //using (var stream = _assembly.GetManifestResourceStream(string.Format($"{path}.{fileName}")))
            //using (var reader = new StreamReader(stream))
            //    text = reader.ReadToEnd();
            return text;
        }

        public static SemanticModel GetSemanticModelForSyntaxTree(SyntaxTree syntaxTree)
        {
            var compilation = CSharpCompilation.Create("temp", syntaxTrees: new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            return semanticModel;
        }
    }
}
