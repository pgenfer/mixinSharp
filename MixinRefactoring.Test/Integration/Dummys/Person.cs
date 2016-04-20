using System;
using System.Collections.Generic;

#pragma warning disable 169
#pragma warning disable 649

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

    /// <summary>
    /// class with a mixin with a static property.
    /// The property should not be added to the child.
    /// </summary>
    public class PersonWithStaticMixin
    {
        private StaticName _name;
    }

    /// <summary>
    /// this class has a mixin which has a
    /// property with a generic parameter.
    /// </summary>
    public class PersonWithGenericMixin
    {
        private INameWithGenericParameter _name;
    }

    /// <summary>
    /// a class that combines a property and a method
    /// </summary>
    public class WorkingPerson
    {
        public string Name { get; set; }
        public void Work(int toolNumber) { }
    }

    /// <summary>
    /// this class has a mixin but implements already 
    /// one property of the mixin, so only
    /// two other properties will be delegated
    /// </summary>
    public class PersonWithFullName
    {
        private Name _name;
        public string FullName => "Already has a fullname property";
    }

    /// <summary>
    /// this class should also implement
    /// the ToString method from the mixin although
    /// it is already defined in the object base class
    /// </summary>
    public class PersonWithToString
    {
        private MixinWithToString _toString;
    }

    /// <summary>
    /// class that already has a method that should not be implemented
    /// </summary>
    public class PersonWithWorkMethod
    {
        private Worker _worker;
        public void Work() { }
    }

    /// <summary>
    /// class that already has a method that should not be implemented
    /// </summary>
    public class PersonWithOtherWorkMethod
    {
        private Worker _worker;
        public void Work(string nameOfWork) { }
    }

    public class PersonWithStaticMethodMixin
    {
        private MixinWithStaticMethod _worker;
    }

    /// <summary>
    /// a child class that should include a mixin with an indexer
    /// </summary>
    public class PersonWithIndexer
    {
        private CollectionWithIndexer _collection;
    }

    /// <summary>
    /// this class uses a mixin with a generic type parameter.
    /// All methods of the mixin should replace the
    /// generic parameter with the real one.
    /// </summary>
    public class PersonWithGenericClassMixin
    {
        private GenericWorker<int> _worker = new GenericWorker<int>();
    }

    /// <summary>
    /// the mixin of this class has a base,
    /// so both methods from base and derived
    /// should be added to the child class
    /// </summary>
    public class PersonWithDerivedWorker
    {
        private DerivedWorker _worker;
    }

    /// <summary>
    /// base class of a person that already implements
    /// a method
    /// </summary>
    public class PersonBase
    {
        // additional work is already impelmented
        // so it should not be taken from the mixin anymore
        public void AdditionalWork() { }
    }

    /// <summary>
    /// derived class that has a base class which already
    /// implements a method from the mixin
    /// </summary>
    public class DerivedPerson : PersonBase
    {
        private DerivedWorker _worker;
    }

    /// <summary>
    /// this class has a lambda method.
    /// When the classes source code is parsed
    /// the "x" parameter in the lambda should
    /// not be handled as method parameter
    /// </summary>
    public class PersonWithLambdaMethod
    {
        private Worker _worker;

        public void MethodWithoutLambda(int methodParameter)
        {
        }

        public void LambdaMethod()
        {
            Action<int> action = x => System.Console.WriteLine(x);
        }
    }

    /// <summary>
    /// base class with an abstract method
    /// </summary>
    public abstract class PersonWithAbstractWork
    {
        public abstract void Work();
    }

    /// <summary>
    /// a class with an abstract method that has the same signature
    /// as the mixin method.
    /// But the method of the mixin should not be implemented,
    /// because only methods from base should be checked.
    /// </summary>
    public abstract class PersonWithAbstractMethod
    {
        private Worker _worker;
        public abstract void Work();
    }

    /// <summary>
    /// class that already overrides a method from base.
    /// The mixins method with the same signature should not be implemented again
    /// </summary>
    public class PersonWithOverriddenMethod : PersonWithAbstractWork
    {
        private Worker _worker;
        public override void Work() { }
    }

    /// <summary>
    /// class with an abstract property
    /// </summary>
    public abstract class PersonWithAbstractName
    {
        public abstract string Name { get; set; }
    }

    /// <summary>
    /// class that overrides a property from base with same
    /// signature as mixin property
    /// </summary>
    public class PersonWithOverriddenProperty : PersonWithAbstractName
    {
        private SimpleName _name;
        public override string Name { get; set; }
    }

    /// <summary>
    /// this class has only native data types
    /// and therefore no mixins should be implemented
    /// </summary>
    public class PersonWithNativeTypes
    {
        private int _int;
        private string _string;
        private byte _byte;
        private double _double;
        private decimal _decimal;
        private IEnumerable<int> _enumerable;
    }

    /// <summary>
    /// class has no regions, so a region should be created
    /// </summary>
    public class PersonWithoutRegion
    {
        private SimpleName _name;
    }

    /// <summary>
    /// class has a region and the region already has a member
    /// add members to existing region
    /// </summary>
    public class PersonWithRegion
    {
        private SimpleName _name;

        #region mixin _name

        /// <summary>
        /// region already has a member
        /// </summary>
        private string _aMember;

        #endregion
    }

    /// <summary>
    /// class has several nested regions,
    /// new members should be added to existing class
    /// </summary>
    public class PersonWithNestedRegions
    {
        private SimpleName _name;

        #region outer region

            #region inner region
            #endregion

            #region mixin _name

            /// <summary>
            /// region already has a member
            /// </summary>
            private string _aMember;

            #endregion

        #endregion
    }

    /// <summary>
    /// class has several nested regions,
    /// but no region with the same name as the created one
    /// so it should be created
    /// </summary>
    public class PersonWithNestedRegionsButWithoutMixinRegion
    {
        private SimpleName _name;

        #region outer region

        #region inner region
        #endregion
        #endregion
    }

    /// <summary>
    /// region exists but is empty
    /// </summary>
    public class PersonWithEmptyRegion
    {
        private SimpleName _name;

        #region mixin _name
        #endregion
    }

    /// <summary>
    /// this class has already one mixin with a region
    /// the second one should be added by the test
    /// </summary>
    public class PersonWithTwoMixins
    {
        private NameMixin _name;
        private Worker _worker;

        #region mixin _name
        public string Name
        {
            get
            {
                return _name.Name;
            }

            set
            {
                _name.Name = value;
            }
        }

        public override string ToString() => _name.ToString();
        #endregion
    }
}
