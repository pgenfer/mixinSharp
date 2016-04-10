using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// class that represents the reference to
    /// a mixin with a mixin child
    /// </summary>
    public class MixinReference
    {
        private NameMixin _name = new NameMixin();
        public ClassWithTypeSymbol Class { get; }

        public MixinReference(string name, ClassWithTypeSymbol type)
        {
            Name = name;
            Class = type;
        }

        public string Name
        {
            get { return _name.Name; }
            private set { _name.Name = value; }
        }

        public override string ToString() => _name.ToString();
    }
}
