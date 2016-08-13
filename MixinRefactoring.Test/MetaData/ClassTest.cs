using NUnit.Framework;
using MixinRefactoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    public class ClassTest : IntegrationTestBase
    {
        [TestDescription("Ensure that event members are returned when iterating over a class' members")]
        public void MembersFromThisAndBase_ContainsEvents()
        {
            WithSourceFiles(Files.ChildClass);
            var @class = CreateClass(nameof(ChildClassWithEvent));

            var members = @class.MembersFromThisAndBase;

            Assert.IsNotNull(members.Single(x => x is Event));
        }
    }
}