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

        public bool IsReadOnly => HasGetter && !HasSetter;

        protected override Member CreateCopy()
        {
            var copy = new Property(Name, Type, HasGetter, HasSetter);
            return copy;
        }
    }
}