using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NUnit.Framework.Assert;

namespace MixinRefactoring.Test
{
    public class NameMixinTest : IntegrationTestBase
    {
        [TestDescription("Check that names with a reserved keyword are prefixed")]
        public void NameIsReservedKeyword_Prefix()
        {
            var mixin = new NameMixin {Name = "new"};
            AreEqual("@new", mixin.Name);
        }

        [TestDescription("Check that names without a reserved keyword are not prefixed")]
        public void NameIsNotReservedKeyword_DoNotPrefix()
        {
            var mixin = new NameMixin { Name = "old" };
            AreEqual("old", mixin.Name);
        }
    }
}