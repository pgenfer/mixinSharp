namespace MixinRefactoring.Test
{
    /// <summary>
    /// class that contains some mixins
    /// </summary>
    public class Person
    {
        /// <summary>
        /// this attribute will be used
        /// to create a mixin
        /// </summary>
        private Name _name;
        /// <summary>
        /// this attribute can be used to include
        /// a mixin with a method
        /// </summary>
        private Worker _worker;
        /// <summary>
        /// A mixin can also be an interface
        /// </summary>
        private IName _interfaceName;
    }

    /// <summary>
    /// the mixin of this child has only a getter
    /// </summary>
    public class PersonWithGetterName
    {
        private OnlyGetterName _name;
    }

    /// <summary>
    /// the property of this included mixin
    /// has only an expression body
    /// </summary>
    public class PersonWithExpressionBodyName
    {
        private ExpressionBodyName _name;
    }

    /// <summary>
    /// This person has already a name,
    /// the mixin should not add another name
    /// </summary>
    public class PersonWithName
    {
        private OnlyGetterName _name;
        public string Name { get; set; }
    }

    /// <summary>
    /// The base class already has a property,
    /// the derived class with the mixin should
    /// not reimplement this property
    /// </summary>
    public class FirstPersonBaseClass
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// derived class with a mixin.
    /// The Name property of the mixin
    /// should not be implemented because it is already
    /// in the base class
    /// </summary>
    public class DerivedPersonClass : FirstPersonBaseClass
    {
        private OnlyGetterName _name;
    }

    /// <summary>
    /// class derived from interface. Since there is no implementation
    /// of the property, it should be included from the mixin
    /// </summary>
    public class DerivedFromInterfaceClass : IName
    {
        private OnlyGetterName _name;
    }

    /// <summary>
    /// this class adds another hierarchy level
    /// to the class hierarchy. The class which is
    /// derived from this should still have the property 
    /// from the base, so no need for implementing the mixins property
    /// </summary>
    public class SecondPersonBaseClass : FirstPersonBaseClass
    {        
    }

    /// <summary>
    /// derived class with two base classes,
    /// the property should be inherited from the first base class
    /// </summary>
    public class ThirdPersonClass : SecondPersonBaseClass
    {
        private OnlyGetterName _name;
    }
}
