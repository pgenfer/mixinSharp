using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    public class MethodSyntaxReaderTest
    {
        [Test]
        public void ClassWithMethodWithoutParameter_Read_MethodRead()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode(Files.Worker);
            var workerClass = sourceCode.Class(nameof(Worker));

            var methodList = new MethodList();

            var methodReader = new MethodSyntaxReader(methodList, sourceCode.Semantic);
            methodReader.Visit(workerClass);            

            Assert.AreEqual(1, methodList.Count);
            Assert.AreEqual("void Work()", methodList[0].ToString());
        }

        [Test]
        public void ClassWithMethodWithParameter_Read_MethodRead()
        {
            // arrange
            // 1. load source files and get class and mixin declarations
            var sourceCode = new SourceCode(Files.Worker);
            var workerClass = sourceCode.Class(nameof(WorkerWithTool));

            var methodList = new MethodList();

            var methodReader = new MethodSyntaxReader(methodList, sourceCode.Semantic);
            methodReader.Visit(workerClass);

            Assert.AreEqual(1, methodList.Count);
            Assert.AreEqual("void Work(int toolNumber)", methodList[0].ToString());
        }
    }
}
