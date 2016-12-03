using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using static NUnit.Framework.Assert;
using static NSubstitute.Substitute;

namespace MixinRefactoring.Test
{
    public class ParameterTest : IntegrationTestBase
    {
        // ensure that this issue is fixed:
        // https://github.com/pgenfer/mixinSharp/issues/23
        [TestDescription("Ensure that parameter name is prefixed")]
        public void Parameter_Prefix_Name()
        {
            var parameter = new Parameter("double",For<ITypeSymbol>());
            AreEqual("@double", parameter.Name);
        }
    }
}