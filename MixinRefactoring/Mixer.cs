using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    /// <summary>
    /// the mixers includes a mixin into a target (the mixin child)
    /// by adding an implementation of every mixin member
    /// to the mixin child
    /// </summary>
    public class Mixer
    {
        private MemberComparer _memberCompare = new MemberComparer();
        private List<Member> _membersToImplement = new List<Member>();
        public Mixer()
        {
        }

        public void IncludeMixinInChild(MixinReference mixin, ClassWithSourceCode child)
        {
            var childMembers = child.MembersFromThisAndBase;
            var mixinMembers = mixin.Class.MembersFromThisAndBase;
            _membersToImplement.AddRange(mixinMembers.Where(x => !childMembers.Any(y => _memberCompare.IsImplementationOf(x, y))));
        }

        public IEnumerable<Member> MembersToImplement => _membersToImplement;
        public IEnumerable<Property> PropertiesToImplement => _membersToImplement.OfType<Property>();
        public IEnumerable<Method> MethodsToImplement => _membersToImplement.OfType<Method>();
    }
}