using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring.Test
{
    /// <summary>
    /// A class that stores some name information
    /// </summary>
    public class Name
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => string.Format($"{FirstName} {LastName}");
    }

    /// <summary>
    /// A class with a single property
    /// </summary>
    public class SimpleName
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// Mixin that has only a readonly property
    /// </summary>
    public class OnlyGetterName
    {
        private string _name;
        public string Name { get { return _name; } }
    }

    /// <summary>
    /// class that has only a property with expression body
    /// </summary>
    public class ExpressionBodyName
    {
        public string Name => "Only expression body";
    }

    /// <summary>
    /// interface with a getter property
    /// </summary>
    public interface IName
    {
        string Name { get; }
    }

    /// <summary>
    /// This is a mixin with a static property,
    /// the property should not be included
    /// into the child
    /// </summary>
    public class StaticName
    {
        public static string Name { get; set; }
    }

    /// <summary>
    /// an interface that has a property with
    /// a generic type parameter
    /// </summary>
    public interface INameWithGenericParameter
    {
        IEnumerable<string> Names { get; set; }
    }   
}
