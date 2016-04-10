using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// this file contains test dummies for testing the constructor functionality
/// </summary>
namespace MixinRefactoring.Test
{
    /// <summary>
    /// a constructor for this class will be generated
    /// </summary>
    public class ChildClassWithoutConstructor
    {
        private Name _name;
    }

    public class ChildClassWithStaticConstructor
    {
        private Name _name;

        static ChildClassWithStaticConstructor()
        { }
    }
    
    /// <summary>
    /// class that has the default constructor implemented
    /// </summary>
    public class ChildClassWithConstructor
    {
        private Name _name;
        /// <summary>
        /// mixin will be injected into this constructor
        /// </summary>
        public ChildClassWithConstructor()
        {
        }
    }

    /// <summary>
    /// class with an additional constructor parameter
    /// </summary>
    public class ChildClassWithConstructorParameter
    {
        private Name _name;
        /// <summary>
        /// constructor will be extended
        /// </summary>
        /// <param name="parameter"></param>
        public ChildClassWithConstructorParameter(int parameter)
        {
        }
    }

    /// <summary>
    /// class has two constructors, the
    /// mixin injection should be delegated to all constructors
    /// </summary>
    public class ChildClassWithContructorInitializer
    {
        private Name _name;

        public ChildClassWithContructorInitializer()
        {
        }

        public ChildClassWithContructorInitializer(int parameter):this()
        {

        }
    }

    /// <summary>
    /// class has a constructor initializer which already has a mixin,
    /// so nothing should be added here
    /// </summary>
    public class ChildWithMixinConstructorInitializer
    {
        private Name _name;
        public ChildWithMixinConstructorInitializer(Name _name)
        { }

        public ChildWithMixinConstructorInitializer(int parameter, Name name) : this(name)
        { }
    }

    /// <summary>
    /// this class already has an injection, so nothing more
    /// should be generated
    /// </summary>
    public class ChildClassWithInjectedMixin
    {
        private Name _name;
        
        public ChildClassWithInjectedMixin(Name name)
        {
            _name = name;
        }
    }
}
