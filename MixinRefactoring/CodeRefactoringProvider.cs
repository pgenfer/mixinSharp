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

            var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            CompositeCommand command = null;
            
            // create mixin from base type
            var baseTypeSyntax = node as SimpleBaseTypeSyntax;
            // create command depending on the selected node
            if (baseTypeSyntax != null)
            {
                command = new CreateMixinFromInterfaceCommand(baseTypeSyntax, model);
            }
            else
            {
                var fieldDeclarationNode = GetContainingFieldDeclaration(node);
                if (fieldDeclarationNode != null)
                    command = new CreateMixinFromFieldDeclarationCommand(fieldDeclarationNode, model);
            }

            if (command == null)
                return;

            // get service provider and read settings from storage
            var serviceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            var settings = new Settings(serviceProvider);

            var childClassDeclaration = node.FindContainingClass(); ;
            var childClass = new ClassFactory(model).Create(childClassDeclaration);
            if (!command.CanExecute(childClass, settings))
                return;
            var action = CodeAction.Create(
                command.Title,
                c => CreateMixin(command, context.Document, childClass, c));
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
            ClassWithSourceCode childClass,
            CancellationToken cancellationToken)
        {
            // get service provider and read settings from storage
            var serviceProvider = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            var settings = new Settings(serviceProvider);

            var model = await document.GetSemanticModelAsync(cancellationToken);
           
            if (mixinCommand.CanExecute(childClass,settings))
            {
                // execute the command => we get a new class declaration
                var newClassDeclaration = mixinCommand.Execute(childClass.SourceCode, model,settings);
                // replace the old class declaration in the syntax tree
                var root = await model.SyntaxTree.GetRootAsync(cancellationToken);
                var newRoot = root.ReplaceNode(childClass.SourceCode, newClassDeclaration);
                // create a new document from the new syntax tree
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument.Project.Solution;
            }

            return null;
        }
    }
}