using Microsoft.CodeAnalysis;



namespace MixinRefactoring
{
    public abstract class SemanticTypeReaderBase
    {
        /// <summary>
        /// TODO: improve this:
        /// there should be a classsymbol reader which
        /// calls the other readers. Because currently this
        /// method is implemented by every reader although 
        /// the reader will only handle the symbols for which it is
        /// responsible
        /// </summary>
        /// <param name="typeSymbol"></param>
        protected virtual void ReadSymbol(ITypeSymbol typeSymbol)
        {
            var properties = typeSymbol.GetMembers().OfType<IPropertySymbol>();
            foreach (var property in properties) ReadSymbol(property);
            var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>();
            foreach (var method in methods) ReadSymbol(method);
            var events = typeSymbol.GetMembers().OfType<IEventSymbol>();
            foreach (var @event in events) ReadSymbol(@event);
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
        protected virtual void ReadSymbol(IMethodSymbol methodSymbol) { }
        protected virtual void ReadSymbol(IParameterSymbol parameter) { }
        protected virtual void ReadSymbol(IEventSymbol @event) { }

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
