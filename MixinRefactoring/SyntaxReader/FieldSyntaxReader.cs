using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    public class FieldSyntaxReader : SyntaxWalkerWithSemantic
    {
        /// <summary>
        /// string constant used to identify the CLR module by name
        /// (the place where the system types are defined)
        /// </summary>
        private const string CommonLanguageRuntimeLibrary = "CommonLanguageRuntimeLibrary";

        public MixinReference MixinReference { get; private set; }

        public FieldSyntaxReader(SemanticModel semantic):base(semantic)
        {           
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            // we take only the first declaration we found
            // so if this field definition has more than one variable declaration,
            // skip after the first one
            if (MixinReference != null)
                return;
            var fieldSymbol = (IFieldSymbol)_semantic.GetDeclaredSymbol(node);
            var typeOfField = fieldSymbol.Type;
            // type could not be resolved => return here
            if (typeOfField.TypeKind == TypeKind.Error)
                return;
            
            // also ignore native types (like int, string, double etc...)
            // we identify them by checking if the types are declared in the runtime itself
            // if yes, they are system types and we will skip them
            var module = typeOfField.ContainingModule;
            if (module != null && module.Name == CommonLanguageRuntimeLibrary)
                return;          

            var classFactory = new ClassFactory(_semantic);
            // create the mixin reference, that is the name of the field and the type the field references
            MixinReference = new MixinReference(fieldSymbol.Name, classFactory.Create(typeOfField));
        }
    }
}
