using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public class Parameter
    {
        public ITypeSymbol Type { get; private set; }
        public string Name { get; private set; }

        public Parameter(string name, ITypeSymbol type)
        {
            Name = name;
            Type = type;
        }

        public bool IsEqual(Parameter other) => Name == other.Name && Type == other.Type;

        public override string ToString() => string.Format("{0} {1}", Type.Name, Name);
    }
}
