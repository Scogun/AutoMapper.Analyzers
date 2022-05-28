using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapper.Analyzers.Common;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullSubstituteCodeFixProvider)), Shared]
public class NullSubstituteCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(NullSubstituteAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var declaration = root.FindToken(diagnosticSpan.Start).Parent.Ancestors().OfType<InvocationExpressionSyntax>()
            .First();

        context.RegisterCodeFix(
            CodeAction.Create("Fix manual null checking", c => UseNullSubstituteAsync(context.Document, declaration, c),
                "CodeFixTitle"), diagnostic);
    }

    private async Task<Document> UseNullSubstituteAsync(Document document, InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken)
    {
        var nullSubstituteInvocationPattern = $"{{0}}.{nameof(IMemberConfigurationExpression.NullSubstitute)}";

        var mapFromInvocationPattern = $"{{0}}.{nameof(IMemberConfigurationExpression.MapFrom)}";

        var optIdentifier = ((invocation.Expression as MemberAccessExpressionSyntax).Expression as IdentifierNameSyntax)
            .Identifier;

        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var invocationNode = syntaxRoot.FindNode(invocation.Span);

        var isComplexMapping = IsComplexMapping(invocation, out var otherValue, out var srcProperty);
        var newNullSubstituteExpression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.ParseExpression(string.Format(nullSubstituteInvocationPattern, optIdentifier)),
            SyntaxFactory.ParseArgumentList($"({otherValue})"));

        if (isComplexMapping)
        {
            var srcName = srcProperty.Substring(0, srcProperty.IndexOf('.'));
            var lambdaExpression = SyntaxFactory.SimpleLambdaExpression(
                SyntaxFactory.Parameter(SyntaxFactory.ParseToken(srcName)), null,
                SyntaxFactory.ParseExpression(srcProperty));
            var newMapFromExpression = SyntaxFactory.InvocationExpression(
                SyntaxFactory.ParseExpression(string.Format(mapFromInvocationPattern, optIdentifier)),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(lambdaExpression))));
            var newBlock = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(newNullSubstituteExpression),
                SyntaxFactory.ExpressionStatement(newMapFromExpression));
            lambdaExpression =
                SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(optIdentifier), newBlock, null);
            return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(invocationNode.Parent, lambdaExpression).NormalizeWhitespace());
        }

        return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(invocationNode, newNullSubstituteExpression).NormalizeWhitespace());
    }

    private bool IsComplexMapping(InvocationExpressionSyntax invocation, out string value, out string srcProperty)
    {
        value = string.Empty;
        srcProperty = string.Empty;

        var syntaxNode = invocation.DescendantNodes()
            .FirstOrDefault(n => n is ConditionalExpressionSyntax || n is BinaryExpressionSyntax);
        if (syntaxNode is ConditionalExpressionSyntax { Condition: BinaryExpressionSyntax binaryCondition } conditional)
        {
            var srcMember = binaryCondition.Left.ToString();
            if (binaryCondition.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
            {
                value = conditional.WhenTrue.ToString();
                srcProperty = conditional.WhenFalse.ToString();
            }

            if (binaryCondition.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
            {
                value = conditional.WhenFalse.ToString();
                srcProperty = conditional.WhenTrue.ToString();
            }

            return !srcMember.Equals(srcProperty);
        }

        if (syntaxNode is BinaryExpressionSyntax binarySyntax &&
            binarySyntax.OperatorToken.IsKind(SyntaxKind.QuestionQuestionToken))
        {
            value = binarySyntax.Right.ToString();
            return false;
        }

        throw new Exception();
    }
}