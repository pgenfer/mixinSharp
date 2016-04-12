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
            // Only offer a refactoring if the selected node is a type declaration node.
            var fieldDeclarationNode = GetContainingFieldDeclaration(node);
            if (fieldDeclarationNode == null)
                return;
            var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            // check if mixin could be executed
            var mixinCommand = new MixinCommand(model, fieldDeclarationNode);
            
            // get service provider and read settings from storage
            var serviceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            var settings = new Settings(serviceProvider);

            if (!mixinCommand.CanExecute(settings))
                return;
            var action = CodeAction.Create(
                $"Include mixin: '{mixinCommand.MixinFieldName}'", 
                c => CreateMixin(context.Document, mixinCommand, c));
            // Register this code action.
            context.RegisterRefactoring(action);
        }

        private FieldDeclarationSyntax GetContainingFieldDeclaration(SyntaxNode node)
        {
            while (node != null && !(node is FieldDeclarationSyntax))
                node = node.Parent;
            return node as FieldDeclarationSyntax;
        }

        private async Task<Solution> CreateMixin(Document document, MixinCommand mixinCommand, CancellationToken cancellationToken)
        {
            // get service provider and read settings from storage
            var serviceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            var settings = new Settings(serviceProvider);

            var model = await document.GetSemanticModelAsync(cancellationToken);
            if (mixinCommand.CanExecute(settings))
            {
                var newClassDeclaration = mixinCommand.Execute(model,settings);
                var root = await model.SyntaxTree.GetRootAsync(cancellationToken);
                var newRoot = root.ReplaceNode(mixinCommand.OriginalChildSource, newClassDeclaration);
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument.Project.Solution;
            }

            return null;
        }
    }
}