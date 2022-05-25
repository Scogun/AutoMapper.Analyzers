using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapper.Analyzers.Common;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ForMemberCodeFixProvider)), Shared]
public class ForMemberCodeFixProvider : CodeFixProvider
{
    private readonly Dictionary<string, string> _fixTitles = new Dictionary<string, string>
    {
        {MapFromAnalyzer.DiagnosticId, "Fix manual mapping for similar name properties"}
    };
    
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MapFromAnalyzer.DiagnosticId);
    
    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
    
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var declaration = root.FindToken(diagnosticSpan.Start).Parent.Ancestors().OfType<InvocationExpressionSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(_fixTitles[diagnostic.Id], c => RemoveBadForMember(context.Document, declaration, c), "ForMemberFixTitle"), diagnostic);
    }

    private async Task<Document> RemoveBadForMember(Document document, InvocationExpressionSyntax declaration, CancellationToken cancellationToken)
    {
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var invocationNode = syntaxRoot.FindNode(declaration.Span);

        var replacer = invocationNode.DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();


        return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(declaration, replacer));
    }
}