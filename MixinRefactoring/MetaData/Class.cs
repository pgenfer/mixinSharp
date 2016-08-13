﻿using System.Collections.Generic;
using System.Linq;

namespace MixinRefactoring
{
    /// <summary>
    /// a class represents an class declaration that 
    /// was either read from source code or from
    /// a type symbol
    /// </summary>
    public class Class : IMethodList, IPropertyList, IEventList
    {
        private readonly PropertyList _properties = new PropertyList();
        private readonly MethodList _methods = new MethodList();
        private readonly EventList _events = new EventList();
        private readonly NameMixin _name = new NameMixin();
        private readonly InterfaceList _interfaces = new InterfaceList();

        public void AddProperty(Property newProperty)
        {
            _properties.AddProperty(newProperty);
            newProperty.Class = this;
        }

        public void AddMethod(Method newMethod)
        {
            _methods.AddMethod(newMethod);
            newMethod.Class = this;
        }

        public virtual InterfaceList Interfaces => _interfaces;
        public IEnumerable<Property> Properties => _properties;
        public IEnumerable<Method> Methods => _methods;
        public IEnumerable<Event> Events => _events;

        public string Name
        {
            get { return _name.Name; }
            set { _name.Name = value; }
        }

        public Class BaseClass
        {
            get;
            set;
        }

        public override string ToString() => _name.ToString();
        public IEnumerable<Member> MembersFromThisAndBase
        {
            get
            {
                var members = new List<Member>();
                members.AddRange(Properties);
                members.AddRange(Methods);
                members.AddRange(Events);
                if (BaseClass != null)
                    members.AddRange(BaseClass.MembersFromThisAndBase);
                return members;
            }
        }

        public bool HasOverride(Member abstractMember)
        {
            if (!abstractMember.IsAbstract)
                return false;
            var memberComparer = new MemberComparer();
            // check if we have a member with the same signature
            // but with the override keyword
            var sameMembers = MembersFromThisAndBase
                .Where(x => x.IsOverride)
                .Where(x => !x.IsAbstract)
                .Where(x => memberComparer.IsSameAs(x, abstractMember));
            return sameMembers.Any();
        }

        public void AddEvent(Event @event) => _events.AddEvent(@event);
    }
}