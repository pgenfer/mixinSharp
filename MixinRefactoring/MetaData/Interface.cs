using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    /// <summary>
    /// representation of an interface
    /// that is implemented by a class.
    /// Currently, only the name of the interface
    /// is important to us
    /// </summary>
    public class Interface
    {
        private readonly NameMixin _name = new NameMixin();
        private readonly ITypeSymbol _interfaceType;

        public Interface(ITypeSymbol interfaceType)
        {
            _interfaceType = interfaceType;
            Name = _interfaceType.Name;
        }

        public string Name
        {
            get { return _name.Name; }
            set { _name.Name = value; }
        }

        public string GetReducedTypeName(SemanticModel semantic,int positionInClass)
        {
            return _interfaceType.ReduceQualifiedTypeName(semantic, positionInClass);
        }

        public override string ToString() => _name.ToString();
    }
}