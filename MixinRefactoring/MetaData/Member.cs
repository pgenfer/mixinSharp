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

        /// <summary>
        /// method is declared as abstract
        /// </summary>
        public bool IsAbstract { get; set; }
        /// <summary>
        /// when generating the method, an override keyword
        /// is necessary
        /// </summary>
        public bool NeedsOverrideKeyword { get; private set; }
        /// <summary>
        /// creates a copy of the given instance, 
        /// should be implemented by derived classes
        /// </summary>
        /// <returns></returns>
        protected abstract Member CreateCopy();
        /// <summary>
        /// creates a copy of this member instance
        /// </summary>
        /// <param name="needsOverrideKeywork">when set, the copy
        /// will have an additional override keyword</param>
        /// <returns></returns>
        public Member Clone(bool needsOverrideKeywork = false)
        {
            var copy = CreateCopy();
            copy.Name = Name;
            copy.NeedsOverrideKeyword = needsOverrideKeywork;
            // if this members will have an override keyword, it cannot be abstract
            copy.IsAbstract = needsOverrideKeywork ? false : IsAbstract;
            return copy;
        }
    }
}
