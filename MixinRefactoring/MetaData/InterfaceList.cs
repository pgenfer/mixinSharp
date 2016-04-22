using System.Collections;
using System.Collections.Generic;

namespace MixinRefactoring
{
    public class InterfaceList : IEnumerable<Interface>
    {
        private List<Interface> _interfaces = new List<Interface>();
        public void AddInterface(Interface @interface) => _interfaces.Add(@interface);
        public IEnumerator<Interface> GetEnumerator() => _interfaces.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _interfaces.GetEnumerator();
    }
}