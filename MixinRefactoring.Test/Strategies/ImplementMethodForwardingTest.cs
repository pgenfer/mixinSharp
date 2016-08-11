using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using NSubstitute;
using NUnit.Framework;

namespace MixinRefactoring.Test
{
    public class ImplementMethodForwardingTest : IntegrationTestBase
    {
        [TestDescription("Check that internal methods from another assembly are not available")]
        public void InternalMethodInExternalAssembly_MethodCannotBeAccessed()
        {
            WithExternalAssemblyFromType(typeof(ExternalClass));
            var mixin = CreateMixinReference("mixin", nameof(MixinWithInternalMethod));
            // the internal method should not be visible
            Assert.IsEmpty(mixin.Class.Methods);
        }
    }
}
