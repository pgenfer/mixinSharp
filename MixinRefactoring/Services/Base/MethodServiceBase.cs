using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// Represents information about a service represented
    /// by a method of the mixin.
    /// Following features are not supported yet:
    /// 1. Default parameters
    /// 2. Methods with generic parameters
    /// 3. ref parameters
    /// </summary>
    public abstract class MethodServiceBase : ServiceBase
    {
        public ITypeSymbol ReturnType { get; protected set; }

        protected List<Parameter> _parameters = new List<Parameter>();

        public MethodServiceBase(string name) : base(name)
        {
        }

        public bool IsEqual(MethodServiceBase otherMethod)
        {
            var equal = Name == otherMethod.Name &&
                        ReturnType == otherMethod.ReturnType &&
                        _parameters.Count == otherMethod._parameters.Count;
            if (!equal)
                return false;
            for (int i = 0; i < _parameters.Count; i++)
                if (!_parameters[i].IsEqual(otherMethod._parameters[i]))
                    return false;
            return true;
        }
    }
}
