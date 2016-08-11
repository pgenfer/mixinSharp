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

    /// <summary>
    /// this child class has a mixin
    /// with hidden members. No delegation code 
    /// should be generated
    /// </summary>
    public class ChildClassWithHiddenPropertyMixin
    {
        private MixinWithHiddenProperties _mixin;
    }

    /// <summary>
    /// this child class has a mixin
    /// with hidden methods. No delegation code 
    /// should be generated
    /// </summary>
    public class ChildClassWithHiddenMethodMixin
    {
        private MixinWithHiddenMethods _mixin;
    }

    /// <summary>
    /// class has a mixin where only the getter of a property is public
    /// </summary>
    public class ChildClassWithPrivateSetterMixin
    {
        private MixinWithPrivateAccessor _mixin;
    }
}
