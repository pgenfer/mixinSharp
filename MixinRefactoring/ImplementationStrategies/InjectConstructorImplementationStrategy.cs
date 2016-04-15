using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    /// <summary>
    /// this method injects the mixin as parameter into the
    /// child class constructor.
    /// The logic works by the following rules:
    /// 1. Every constructor in the class will be extended
    /// 2. If a constructor does not have an initialization list
    ///    with a constructor of the same class, the mixin field will be assigned in this
    ///    constructors body.
    /// 3. If the constructor initializer has a constructor of the same class, the parameter
    ///    will be delegated to this constructor.
    /// 4. If the constructor already has a parameter of the same type with the same name, do not add it again
    /// Example:
    /// public class Test
    /// {
    ///     private Mixin _mixin;
    ///     public Test(){}
    ///     public Test(int i):this(){}
    /// }
    /// becomes:
    /// public class Test
    /// {
    ///     private Mixin _mixin;
    ///     public Test(Mixin mixin){_mixin = mixin;}
    ///     public Test(int i,Mixin mixin=null):this(mixin){}
    /// }
    /// In that way, the mixin instance will always be initialized,
    /// no matter which constructor is called.
    /// </summary>
    public class InjectConstructorImplementationStrategy
    {
        /// <summary>
        /// the mixin that will be used to generated the constructor parameter
        /// </summary>
        private readonly MixinReference _mixin;
        // needed to resolve type names correctly
        private readonly int _classPositionInSourceCode;
        // needed to 
        private readonly SemanticModel _semantic;

        public InjectConstructorImplementationStrategy(
            MixinReference mixin,
            SemanticModel semanticModel,
            int positionInSourceFile)
        {
            _mixin = mixin;
            _semantic = semanticModel;
            _classPositionInSourceCode = positionInSourceFile;
        }

        private ParameterSyntax CreateConstructorParameterForMixin(string parameterName)
        {
            var parameter = Parameter(Identifier(parameterName))
                .WithType(ParseTypeName(_mixin.Class.TypeSymbol.ReduceQualifiedTypeName(_semantic, _classPositionInSourceCode)))
                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression)));
            return parameter;
        }

        private ExpressionStatementSyntax CreateAssigmentStatementForConstructorBody(string parameterName)
        {
            var assignment = AssignmentExpression(
                   SyntaxKind.SimpleAssignmentExpression,
                   IdentifierName(_mixin.Name),
                   IdentifierName(parameterName));
            return ExpressionStatement(assignment);
        }
        
        /// <summary>
        /// if the child class already has a constructor, 
        /// a new parameter will be added to the constructor
        /// </summary>
        /// <param name="oldConstructor">constructor to which
        /// we are going to add the new parameter</param>
        /// <returns>the constructor with the additional new parameter</returns>
        public ConstructorDeclarationSyntax ExtendExistingConstructor(
            ConstructorDeclarationSyntax oldConstructor)
        {
            if (oldConstructor.IsStatic())
                return oldConstructor;

            var parameterName = _mixin.Name.ConvertFieldNameToParameterName();
            // if there is already a parameter with the same name, skip further processing
            var alreadyHasParameter = oldConstructor.ParameterList.Parameters.Any(x => x.Identifier.Text == parameterName);
            if (alreadyHasParameter)
                return oldConstructor;

            // first rule: extend the constructors parameter list
            var parameter = CreateConstructorParameterForMixin(parameterName);
            var newConstructor = oldConstructor.AddParameterListParameters(parameter);
            // second rule: check for initializer
            // if we have no initializer or a base initializer, do the assignment in this constructor
            // but do not delegate the parameter to further constructors
            var initializer = oldConstructor.Initializer;
            if(initializer == null || initializer.IsKind(SyntaxKind.BaseConstructorInitializer))
            {
                newConstructor = newConstructor
                    .AddBodyStatements(CreateAssigmentStatementForConstructorBody(parameterName));
            }
            return newConstructor;
        }      
        
        /// <summary>
        /// the constructor initializer will be extended
        /// so that it will accept the mixin as parameter.
        /// See rule 3 from above
        /// </summary>
        /// <param name="oldConstructorInitializer"></param>
        /// <returns></returns>
        public ConstructorInitializerSyntax ExtendConstructorInitialization(
            ConstructorInitializerSyntax oldConstructorInitializer)
        {
            // don't do anything if initializer points to base
            if (oldConstructorInitializer.IsKind(SyntaxKind.BaseConstructorInitializer))
                return oldConstructorInitializer;
            var parameterName = _mixin.Name.ConvertFieldNameToParameterName();

            // arguments that are already used in the constructor initializer
            var arguments = oldConstructorInitializer.ArgumentList.Arguments;

            // the initializer can have default parameters that are not visible in the syntax tree,
            // therefore we have to use some additional semantic information here
            var useArgumentName = false;
            var initalizerSymbol = _semantic.GetSymbolInfo(oldConstructorInitializer).Symbol as IMethodSymbol;
            if (initalizerSymbol != null)
            {
                // new mixin parameter is always added at the end, but if there is a default parameter before
                // that is not set, we must use explicit naming
                var firstDefaultArgument = initalizerSymbol.Parameters.FirstOrDefault(x => x.HasExplicitDefaultValue);
                // we have a default argument and it is not our mixin itself => use explicit naming here
                useArgumentName = 
                    firstDefaultArgument != null &&  // there must be a parameter with possible default argument before
                    firstDefaultArgument.Name != parameterName && // this should not be our new parameter itself
                    !arguments.Any(x => x.GetText().ToString() == firstDefaultArgument.Name); // and the parameter should not be set in the initializer
            }
            // if there is already a parameter with the same name, skip further processing
            var alreadyHasParameter = arguments.Any(x => x.GetText().ToString() == parameterName);
            if (alreadyHasParameter)
                return oldConstructorInitializer;

            var argument = useArgumentName
                ? Argument(NameColon(parameterName), default(SyntaxToken), IdentifierName(parameterName))
                : Argument(IdentifierName(parameterName));

            var newConstructorInitializer = oldConstructorInitializer.AddArgumentListArguments(argument);
                
            return newConstructorInitializer;
        } 

        public ConstructorDeclarationSyntax CreateNewConstructor(string className)
        {
            var parameterName = _mixin.Name.ConvertFieldNameToParameterName();
            var parameter = CreateConstructorParameterForMixin(parameterName);
            var constructor =
                ConstructorDeclaration(className)
                .AddParameterListParameters(parameter)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBodyStatements(CreateAssigmentStatementForConstructorBody(parameterName));
            return constructor;
        }
    }   
}
