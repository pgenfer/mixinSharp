using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public abstract class Member
    {
        private NameMixin _name = new NameMixin();

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
    }
}
