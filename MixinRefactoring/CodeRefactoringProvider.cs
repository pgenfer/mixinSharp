using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Formatting;

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

            // try to extract mixin information from the field declaration
            var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            var mixin = new MixinReferenceFactory(model).Create(fieldDeclarationNode);
            if (mixin == null)
                return;

            var action = CodeAction.Create($"Include mixin: '{mixin.Name}'", c => CreateMixin(context.Document, fieldDeclarationNode, c));

            // Register this code action.
            context.RegisterRefactoring(action);
        }

        private FieldDeclarationSyntax GetContainingFieldDeclaration(SyntaxNode node)
        {
            while (node != null && !(node is FieldDeclarationSyntax))
                node = node.Parent;
            return node as FieldDeclarationSyntax;
        }

        private async Task<Solution> CreateMixin(Document document, FieldDeclarationSyntax fieldDeclarationNode, CancellationToken cancellationToken)
        {
            var model = await document.GetSemanticModelAsync(cancellationToken);

            var classDeclarationNode = fieldDeclarationNode.Parent as ClassDeclarationSyntax;

            var mixin = new MixinReferenceFactory(model).Create(fieldDeclarationNode);
            if (mixin == null)
                return null;

            var mixinChild = new ClassFactory(model).Create(classDeclarationNode);
            if (mixinChild == null)
                return null;

            // do the refactoring
            var mixer = new Mixer();
            mixer.IncludeMixinInChild(mixin, mixinChild);
            var syntaxWriter = new IncludeMixinSyntaxWriter(mixer.MembersToImplement, mixin.Name, model);
            var newClassDeclaration = syntaxWriter.Visit(mixinChild.SourceCode);

            var root = await model.SyntaxTree.GetRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(classDeclarationNode, newClassDeclaration);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument.Project.Solution;
        }
    }
}