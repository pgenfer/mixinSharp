using System.Collections.Generic;
using System.Collections;

namespace MixinRefactoring
{
    public class MethodList : IMethodList, IEnumerable<Method>
    {
        private readonly List<Method> _methods = new List<Method>();
        public void AddMethod(Method newMethod) => _methods.Add(newMethod);
        //public void AddMethods(IEnumerable<Method> methods) => _methods.AddRange(methods);
        public IEnumerator<Method> GetEnumerator() => _methods.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _methods.GetEnumerator();
        public int Count => _methods.Count;
        public Method this[int index] => _methods[index];
    }
}