using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public abstract class PropertyServiceBase : ServiceBase
    {
        public ITypeSymbol Type { get; protected set; }
        public bool HasSetter { get; protected set; }
        public bool HasGetter { get; protected set; }

        public PropertyServiceBase(string name) : base(name)
        {
        }

        public PropertyServiceBase(IPropertySymbol propertySymbol):this(propertySymbol.Name)
        {
            Type = propertySymbol.Type;
            HasGetter = propertySymbol.GetMethod != null &&
                        propertySymbol.GetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public);
            HasSetter = propertySymbol.SetMethod != null &&
                        propertySymbol.SetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public);
        }

        public virtual bool IsEqual(PropertyServiceBase other)
        {
            return Name == other.Name;
        }
    }
}
