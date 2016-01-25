using System.Collections.Generic;

namespace MixinRefactoring
{
    /// <summary>
    /// a class represents an class declaration that 
    /// was either read from source code or from
    /// a type symbol
    /// </summary>
    public class Class : IMethodList, IPropertyList
    {
        private PropertyList _properties = new PropertyList();
        private MethodList _methods = new MethodList();
        private readonly NameMixin _name = new NameMixin();
        public void AddProperty(Property newProperty) => _properties.AddProperty(newProperty);
        public void AddMethod(Method newMethod) => _methods.AddMethod(newMethod);
        public IEnumerable<Property> Properties => _properties;
        public IEnumerable<Method> Methods => _methods;
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
                if (BaseClass != null)
                    members.AddRange(BaseClass.MembersFromThisAndBase);
                return members;
            }
        }
    }
}