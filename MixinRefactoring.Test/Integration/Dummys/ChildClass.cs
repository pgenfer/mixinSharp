using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// this is the easiest child class configuration.
    /// A child class that has only one mixin
    /// without any interfaces or anything special
    /// </summary>
    public class SimpleChildClass
    {
        private SimpleMixin _mixin;
    }

    /// <summary>
    /// this is a simple child class that does not 
    /// have any reference field to a mixin
    /// </summary>
    public class SimpleChildClassWithoutField
    {
    }
}
