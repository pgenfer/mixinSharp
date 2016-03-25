using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    [TestFixture]
    class ParameterSyntaxReaderTest
    {
        /// <summary>
        /// Checks that issue
        /// https://github.com/pgenfer/mixinSharp/issues/3
        /// is resolved
        /// </summary>
        [Test]
        public void ClassDeclarationWithLambda_ReadParameter_IgnoreLambdaParameter()
        {
            var sourceCode = new SourceCode(Files.Person);
            var personClass = sourceCode.Class(nameof(PersonWithLambdaMethod));

            var parameterList = new ParameterList();

            var parameterSyntaxReader = new ParameterSyntaxReader(parameterList, sourceCode.Semantic);
            parameterSyntaxReader.Visit(personClass);

            // the parameter that is used in the lambda expression should not
            // be added to the parameter list, but the method parameter should be
            Assert.AreEqual(1, parameterList.ParameterCount);
            Assert.AreEqual("methodParameter", parameterList.Single().Name);
        }
    }
}
