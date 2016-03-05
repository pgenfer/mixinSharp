using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class MetaDataReferenceResolver
    {
        public static MetadataReference[] ResolveSystemAssemblies()
        {
            return new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)//,
                //MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location)
            };
        }
    }
}
