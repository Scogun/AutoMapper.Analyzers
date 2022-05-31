using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AutoMapper.Analyzers.Common;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FlattingComplexModelCodeFixProvider)), Shared]
public class FlattingComplexModelCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FlattingComplexModelAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpans = new List<TextSpan> { diagnostic.Location.SourceSpan };
        diagnosticSpans.AddRange(diagnostic.AdditionalLocations.Select(al => al.SourceSpan));

        var declarations = diagnosticSpans.Select(s =>
            root.FindToken(s.Start).Parent.Ancestors().OfType<InvocationExpressionSyntax>().First());

        context.RegisterCodeFix(
            CodeAction.Create("Replace manual complex flatting by IncludeMembers call",
                c => UseIncludeMembers(context.Document, declarations, c), "FlattingComplexModelFixTitle"), diagnostic);
    }

    private async Task<Document> UseIncludeMembers(Document document,
        IEnumerable<InvocationExpressionSyntax> declarations, CancellationToken cancellationToken)
    {
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        syntaxRoot = syntaxRoot.ReplaceNodes(declarations.Select(d => syntaxRoot.FindNode(d.Span)),
            (_, syntaxNode) => syntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault());

        var lambda = BuildLambdaExpression(declarations);
        var includeMembers = GetIncludeMembersInvocation(declarations, ref syntaxRoot);

        var argumentList = new List<ArgumentSyntax>(includeMembers.ArgumentList.Arguments) { SyntaxFactory.Argument(lambda) };

        var includeCall = SyntaxFactory.InvocationExpression(includeMembers.Expression,
            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argumentList)));

        return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(includeMembers, includeCall).NormalizeWhitespace());
    }

    private InvocationExpressionSyntax GetIncludeMembersInvocation(IEnumerable<InvocationExpressionSyntax> declarations, ref SyntaxNode? syntaxRoot)
    {
        var invocationExpressions = syntaxRoot.FindToken(declarations.ElementAt(0).SpanStart).Parent.Ancestors().OfType<InvocationExpressionSyntax>();
        var includeMembersName = nameof(IMappingExpression.IncludeMembers);
        var includeMembers = invocationExpressions.FirstOrDefault(i => i.ToString().Contains(includeMembersName));
        if (includeMembers == null)
        {
            SyntaxNode createMap = invocationExpressions.Where(i => i.Expression is GenericNameSyntax).FirstOrDefault(i => i.ToString().StartsWith(nameof(Profile.CreateMap)));
            var newCreateMapString = createMap.Parent.ToFullString().Replace(createMap.ToFullString().Trim(),$"{createMap.ToFullString()}.{includeMembersName}()");
            var newCreateMap = SyntaxFactory.ParseExpression(newCreateMapString);
            if (createMap.Parent is MemberAccessExpressionSyntax)
            {
                createMap = createMap.Parent;
            }
            syntaxRoot = syntaxRoot.ReplaceNode(createMap, newCreateMap);
            includeMembers = syntaxRoot.FindToken(declarations.ElementAt(0).SpanStart).Parent.Ancestors().OfType<InvocationExpressionSyntax>()
                .First(i => i.ToString().Contains(includeMembersName));
        }

        return includeMembers;
    }

    private static SimpleLambdaExpressionSyntax BuildLambdaExpression(IEnumerable<InvocationExpressionSyntax> declarations)
    {
        var srcProperty =
            ForMemberAnalyzer.GetLambdaExpressions(declarations.ElementAt(0)).srcExpression as SimpleLambdaExpressionSyntax;
        var srcCall = srcProperty.ExpressionBody.ToFullString();
        srcCall = srcCall.Substring(0, srcCall.LastIndexOf('.'));
        var srcName = srcProperty.Parameter.Identifier.Text;

        return SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(SyntaxFactory.ParseToken(srcName)),
            SyntaxFactory.ParseExpression(srcCall));
    }
}