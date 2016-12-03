using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class Parameter
    {
        private readonly NameMixin _name = new NameMixin();

        public ITypeSymbol Type
        {
            get;
        }

        public Parameter(string name, ITypeSymbol type)
        {
            Name = name;
            Type = type;
        }

        public bool IsEqual(Parameter other) => Name == other.Name && Type == other.Type;
        public override string ToString() => $"{Type.Name} {Name}";

        public string Name
        {
            get { return _name.Name; }
            set { _name.Name = value; }
        }
    }
}