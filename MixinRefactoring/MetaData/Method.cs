using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MixinRefactoring
{
    public class Method : Member, IParameterList
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="overrideFromObject">
        /// this flag is set if the method was originally declared
        /// in System.Object and is overriden. When the mixin is included,
        /// overriden methods from System.Object should still be added to the child</param>
        public Method(string name, ITypeSymbol type, bool overrideFromObject = false)
        {
            Name = name;
            ReturnType = type;
            IsOverrideFromObject = overrideFromObject;
        }

        private ParameterList _parameters = new ParameterList();
        public ITypeSymbol ReturnType
        {
            get;
        }

        public IEnumerable<Parameter> Parameters => _parameters;
        public void Add(Parameter parameter) => _parameters.Add(parameter);
      
        public int ParameterCount => _parameters.ParameterCount;
        public override string ToString() => $"{ReturnType.ToString()} {Name.ToString()}({_parameters.ToString()})";
        public Parameter GetParameter(int index) => _parameters.GetParameter(index);
        /// <summary>
        /// method is an override from the original method in System.Object
        /// </summary>
        public bool IsOverrideFromObject { get; }
    }
}