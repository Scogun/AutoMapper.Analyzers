using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapper.Analyzers.Common;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CreateMapIntoTryCodeFixProvider)), Shared]
public class CreateMapIntoTryCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CreateMapIntoTryAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var declaration = root.FindToken(diagnosticSpan.Start).Parent.Ancestors().OfType<ExpressionStatementSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create("Extract CreateMap from try expression", c => ExtractCreateMap(context.Document, declaration, c), "CreateMapFixTitle"), diagnostic);
    }

    private async Task<Document> ExtractCreateMap(Document document, ExpressionStatementSyntax declaration, CancellationToken cancellationToken)
    {
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var invocationNode = syntaxRoot.FindNode(declaration.Span);
        syntaxRoot = syntaxRoot.RemoveNode(invocationNode, SyntaxRemoveOptions.KeepDirectives);
        var tryNode = syntaxRoot.DescendantNodes().OfType<TryStatementSyntax>().First();
        syntaxRoot = syntaxRoot.InsertNodesBefore(tryNode, new[] { invocationNode }).NormalizeWhitespace();

        return document.WithSyntaxRoot(syntaxRoot);
    }
}