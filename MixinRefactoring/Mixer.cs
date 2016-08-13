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
        private readonly MemberComparer _memberCompare = new MemberComparer();
        private readonly List<Member> _membersToImplement = new List<Member>();
      
        public void IncludeMixinInChild(MixinReference mixin, ClassWithSourceCode child)
        {
            var childMembers = child.MembersFromThisAndBase;
            var mixinMembers = mixin.Class.MembersFromThisAndBase;
            foreach(var mixinMember in mixinMembers)
            {
                var membersWithSameSignatureInChild = childMembers
                    .Where(x => _memberCompare.IsSameAs(x, mixinMember))
                    .ToList();
                // 1. case: method does not exist in child => implement it
                if (!membersWithSameSignatureInChild.Any())
                    _membersToImplement.Add(mixinMember.Clone());
                else // 2. case: method does exist in child, but is abstract and not overridden => override it
                {
                    // member is declared as abstract in a base class of child
                    // but not in child itself
                    var abstractMembers = membersWithSameSignatureInChild
                        .Where(x => 
                            x.IsAbstract && 
                            x.Class != child && 
                            !child.HasOverride(x));
                    _membersToImplement.AddRange(abstractMembers.Select(x => x.Clone(true)));
                }
            }
        }

        public IEnumerable<Member> MembersToImplement => _membersToImplement;
        public IEnumerable<Property> PropertiesToImplement => _membersToImplement.OfType<Property>();
        public IEnumerable<Method> MethodsToImplement => _membersToImplement.OfType<Method>();
        public IEnumerable<Event> EventsToImplement => _membersToImplement.OfType<Event>();
    }
}