using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public class ImplementedPropertyService : PropertyServiceBase, IImplementedMixinService
    {
        public ImplementedPropertyService(
            PropertyDeclarationSyntax property,
            SemanticModel semanticModel) :
            base(property.Identifier.ToString())
        {
            Type = property.Type.ToTypeSymbol(semanticModel);
            HasGetter =
                property.ExpressionBody != null || // properties with expression bodies do not have an accessorlist
                property.AccessorList.Accessors.Any(x => x.Kind() == SyntaxKind.GetAccessorDeclaration);
            HasSetter =
                property.AccessorList != null &&
                property.AccessorList.Accessors.Any(x => x.Kind() == SyntaxKind.SetAccessorDeclaration);
        }

        public ImplementedPropertyService(IPropertySymbol property):base(property)
        {
        }

        public bool IsImplementationOf(IMixinService service) => service.IsImplementedBy(this);
    }
}
