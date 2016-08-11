using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// this is just a simple mixin class
    /// with a property
    /// </summary>
    public class SimpleMixin
    {
        public string Property { get; set; }
    }

    /// <summary>
    /// an simple interface with one method
    /// </summary>
    public interface IMixinInterface
    {
        void Method();
    }

    /// <summary>
    /// this mixin has members that should not be included
    /// in the child class because they are not
    /// accessible by clients
    /// </summary>
    public class MixinWithHiddenProperties
    {
        private string PrivateProperty { get; set; }
        protected string ProtectedProperty { get; set; }
    }

    /// <summary>
    /// this mixin has members that should not be included
    /// in the child class because they are not
    /// accessible by clients
    /// </summary>
    public class MixinWithHiddenMethods
    {
        private void PrivateMethod() { }
        protected void ProtectedMethod() { }
    }

    /// <summary>
    /// this mixin has a property where only
    /// the getter is accessible,
    /// the property should be generated as readonly
    /// </summary>
    public class MixinWithPrivateAccessor
    {
        public string Property { get; private set; }
    }

    /// <summary>
    /// property is internal and should only be generated
    /// when accessible inside the child class
    /// </summary>
    public class MixinWithInternalProperty
    {
        internal string Property { get; set; }
    }
}

