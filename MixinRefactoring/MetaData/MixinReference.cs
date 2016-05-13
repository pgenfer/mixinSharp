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
        public virtual ClassWithTypeSymbol Class { get; }

        /// <summary>
        /// only for test cases, don't use in production code
        /// </summary>
        public MixinReference()
        { }

        public MixinReference(string name, ClassWithTypeSymbol type)
        {
            Name = name;
            Class = type;
        }

        public virtual string Name
        {
            get { return _name.Name; }
            private set { _name.Name = value; }
        }

        public override string ToString() => _name.ToString();
    }
}
