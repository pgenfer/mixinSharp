using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// used to correctly format the data types.
    /// Implemented as extension method so that
    /// it can be accessed through the IMixinService interface
    /// </summary>
    public static class ITypeSymbolExtension
    {
        private static SymbolDisplayFormat _symbolDisplayFormat =
            new SymbolDisplayFormat(miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static string DisplayFormat(this ITypeSymbol type) => type.ToDisplayString(_symbolDisplayFormat);
    }
}
