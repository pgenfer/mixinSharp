using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public abstract class Member
    {
        private readonly NameMixin _name = new NameMixin();

        /// <summary>
        /// the class where this member is declared.
        /// This information is needed so that we know
        /// if the member is declared in a base class or in the class itself
        /// </summary>
        public Class Class { get; set; }
        
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

        /// <summary>
        /// returns the modifiers as a string for easier usage of to string method
        /// </summary>
        protected string Modifiers => IsOverride ? "override" : IsAbstract ? "abstract" : string.Empty;

        public override string ToString() => _name.ToString();

        /// <summary>
        /// method is declared as abstract
        /// </summary>
        public bool IsAbstract { get; set; }
        /// <summary>
        /// This member has the override keyword set
        /// </summary>
        public bool IsOverride { get; set; }
        /// <summary>
        /// flag that determines whether this member is declared as internal
        /// </summary>
        public virtual bool IsInternal { get; set; }
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
            copy.IsOverride = needsOverrideKeywork;
            copy.Documentation = Documentation; // both hold the same reference, check if this might become a problem
            // if this member will have an override keyword, it cannot be abstract
            copy.IsAbstract = !needsOverrideKeywork && IsAbstract;
            copy.IsInternal = IsInternal;
            return copy;
        }

        /// <summary>
        /// stores the comment of this member (if any)
        /// </summary>
        public DocumentationComment Documentation { get; set; }     
    }
}
