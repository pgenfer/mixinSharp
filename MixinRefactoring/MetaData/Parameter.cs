using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class Parameter
    {
        public ITypeSymbol Type
        {
            get;
        }

        public string Name
        {
            get;
        }

        public Parameter(string name, ITypeSymbol type)
        {
            Name = name;
            Type = type;
        }

        public bool IsEqual(Parameter other) => Name == other.Name && Type == other.Type;
        public override string ToString() => string.Format("{0} {1}", Type.Name, Name);
    }
}