using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    public class AddMixinToBaseListCommandTest : IntegrationTestBase
    {
        [TestDescription(
            @"Test ensures that nothing is added to the child's base 
              list if mixin does not have any interfaces")]
        public void MixinWithoutInterface_Include_NoBaseClassList()
        {
            WithSourceFiles(Files.ChildClass, Files.Mixin);
            var childClass = CreateClass(nameof(SimpleChildClass));
            var mixin = CreateMixinReference(childClass, "_mixin");

            var command = new AddMixinToBaseListCommand(mixin);
            var newClassDeclaration =
                command.Execute(
                    childClass.SourceCode,
                    Semantic,
                    new Settings(addInterfacesToChild: true));

            Assert.IsNull(newClassDeclaration.BaseList);
        }
    }
}
