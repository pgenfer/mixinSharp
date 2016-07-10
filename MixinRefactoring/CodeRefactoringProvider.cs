using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace MixinRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MixinRefactoringCodeRefactoringProvider)), Shared]
    internal class MixinRefactoringCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            // Find the node at the selection.
            var node = root.FindNode(context.Span);

            // create mixin from base type
            var baseTypeSyntax = node as SimpleBaseTypeSyntax;
            if(baseTypeSyntax != null)
            {
                // TODO: create command that takes base type here
            }

            // Only offer a refactoring if the selected node is a type declaration node.
            var fieldDeclarationNode = GetContainingFieldDeclaration(node);

            if (fieldDeclarationNode == null)
                return;
            var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            var childClassDeclaration = (ClassDeclarationSyntax)fieldDeclarationNode.Parent;
            // check if mixin could be executed
            var mixin = new MixinReferenceFactory(model).Create(fieldDeclarationNode);
            var childClass = new ClassFactory(model).Create(childClassDeclaration);
            var mixinCommand = new CreateMixinFromFieldDeclarationCommand(fieldDeclarationNode,model);
            
            // get service provider and read settings from storage
            var serviceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            var settings = new Settings(serviceProvider);

            if (!mixinCommand.CanExecute(childClass, settings))
                return;
            var action = CodeAction.Create(
                $"Include mixin: '{mixin.Name}'", 
                c => CreateMixin(mixinCommand, context.Document,childClassDeclaration, c));
            // Register this code action.
            context.RegisterRefactoring(action);
        }

        private FieldDeclarationSyntax GetContainingFieldDeclaration(SyntaxNode node)
        {
            while (node != null && !(node is FieldDeclarationSyntax))
                node = node.Parent;
            return node as FieldDeclarationSyntax;
        }

        private async Task<Solution> CreateMixin(
            IMixinCommand mixinCommand, 
            Document document,
            ClassDeclarationSyntax childClassDeclaration,
            CancellationToken cancellationToken)
        {
            // get service provider and read settings from storage
            var serviceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            var settings = new Settings(serviceProvider);

            var model = await document.GetSemanticModelAsync(cancellationToken);
           
            var childClass = new ClassFactory(model).Create(childClassDeclaration);
            if (mixinCommand.CanExecute(childClass,settings))
            {
                // execute the command => we get a new class declaration
                var newClassDeclaration = mixinCommand.Execute(childClassDeclaration,model,settings);
                // replace the old class declaration in the syntax tree
                var root = await model.SyntaxTree.GetRootAsync(cancellationToken);
                var newRoot = root.ReplaceNode(childClassDeclaration, newClassDeclaration);
                // create a new document from the new syntax tree
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument.Project.Solution;
            }

            return null;
        }
    }
}