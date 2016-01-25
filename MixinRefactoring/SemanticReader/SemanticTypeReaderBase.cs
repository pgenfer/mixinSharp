using Microsoft.CodeAnalysis;



namespace MixinRefactoring
{
    public abstract class SemanticTypeReaderBase
    {
        protected virtual void ReadSymbol(ITypeSymbol typeSymbol)
        {
            var properties = typeSymbol.GetMembers().OfType<IPropertySymbol>();
            foreach (var property in properties) ReadSymbol(property);
            var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>();
            foreach (var method in methods) ReadSymbol(method);
        }

        /// <summary>
        /// fallback if symbol could not be resolved to any of 
        /// the other types
        /// </summary>
        /// <param name="symbol"></param>
        protected virtual void ReadSymbol(ISymbol symbol) { }

        protected virtual void ReadSymbol(IPropertySymbol propertySymbol) { }
        /// <summary>
        /// when overridden, don't forget to call base method to read parameters
        /// </summary>
        /// <param name="methodSymbol"></param>
        protected virtual void ReadSymbol(IMethodSymbol methodSymbol)
        {
            foreach (var parameter in methodSymbol.Parameters)
                ReadSymbol(parameter);
        }
        protected virtual void ReadSymbol(IParameterSymbol parameter) { }

        public void VisitSymbol(ISymbol symbol)
        {
            // do dynamic dispatching here:
            // the runtime will choose the correct method 
            // depending on the method parameter,
            // if no good match is found the fallback implementation
            // will be called
            ReadSymbol((dynamic)symbol);
        }
    }
}
