using System;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class Property : Member
    {
        public Property(string name, ITypeSymbol type, bool hasGetter, bool hasSetter)
        {
            Name = name;
            Type = type;
            HasSetter = hasSetter;
            HasGetter = hasGetter;
        }

        public ITypeSymbol Type
        {
            get;
        }

        public bool HasSetter
        {
            get;
        }

        public bool HasGetter
        {
            get;
        }

        /// <summary>
        /// store information if the getter can only be 
        /// accessed in the same assembly
        /// </summary>
        public bool IsGetterInternal { get; set; }
        /// <summary>
        /// store information if setter can only be accessed within
        /// the same assembly
        /// </summary>
        public bool IsSetterInternal { get; set; }
        /// <summary>
        /// A  property  is internal if both getter and setter are defined internal
        /// </summary>
        public override bool IsInternal => IsGetterInternal && IsSetterInternal;

        public bool IsReadOnly => HasGetter && !HasSetter;

        protected override Member CreateCopy()
        {
            var copy = new Property(Name, Type, HasGetter, HasSetter)
            {
                IsSetterInternal = IsSetterInternal,
                IsGetterInternal = IsGetterInternal
            };
            return copy;
        }
    }
}