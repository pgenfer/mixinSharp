using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace MixinRefactoring
{
    public class ParameterSymbolReader : SemanticTypeReaderBase
    {
        private readonly IParameterList _parameters;

        public ParameterSymbolReader(IParameterList parameters)
        {
            _parameters = parameters;
        }

        protected override void ReadSymbol(IParameterSymbol parameter) =>
            _parameters.Add(new Parameter(parameter.Name, parameter.Type));


        protected override void ReadSymbol(IPropertySymbol propertySymbol)
        {
            foreach (var parameter in propertySymbol.Parameters)
                ReadSymbol(parameter);
        }
        /// <summary>
        /// when overridden, don't forget to call base method to read parameters
        /// </summary>
        /// <param name="methodSymbol"></param>
        protected override void ReadSymbol(IMethodSymbol methodSymbol)
        {
            foreach (var parameter in methodSymbol.Parameters)
                ReadSymbol(parameter);
        }
    }
}
