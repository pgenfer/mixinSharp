using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// contains all classes for test cases that are not compilable
/// and will only be compilable if the mixins will be generated
/// correctly
/// </summary>
namespace MixinRefactoring.Test
{
    /// <summary>
    /// class derived from interface. Since there is no implementation
    /// of the property, it should be included from the mixin
    /// </summary>
    public class DerivedFromInterfaceClass : IName
    {
        private OnlyGetterName _name;
    }

    /// <summary>
    /// derived class with a mixin,
    /// the method of the mixin should override the abstract method
    /// </summary>
    public class PersonFromAbstractWork : PersonWithAbstractWork
    {
        private Worker _worker;
    }

    /// <summary>
    /// class where an abstract property must be overridden during code 
    /// generation
    /// </summary>
    public class PersonFromAbstractName : PersonWithAbstractName
    {
        private SimpleName _name;
    }
}
