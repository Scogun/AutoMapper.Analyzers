using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NullSubstituteAnalyzer : ForMemberAnalyzer
{
    public const string DiagnosticId = "AMA0003";

    protected override string InternalDiagnosticId => DiagnosticId;

    protected override Diagnostic AnalyzeMapFrom(LambdaExpressionSyntax destExpression,
        LambdaExpressionSyntax srcExpression)
    {
        if (IsManualNotNullChecking(srcExpression, out var srcProperty) &&
            TryGetExpressionMemberName(destExpression, out string destProperty) && destProperty.Equals(srcProperty))
        {
            return Diagnostic.Create(Rule, srcExpression.Parent.Parent.Parent.GetLocation(), ProfileName, MapName);
        }

        return base.AnalyzeMapFrom(destExpression, srcExpression);
    }

    private bool IsManualNotNullChecking(LambdaExpressionSyntax expression, out string propertyName)
    {
        propertyName = string.Empty;

        if (expression.Body is BinaryExpressionSyntax binarySyntax &&
            binarySyntax.OperatorToken.IsKind(SyntaxKind.QuestionQuestionToken) &&
            TryGetExpressionMemberName(binarySyntax.Left, out propertyName))
        {
            return true;
        }

        if (expression.Body is ConditionalExpressionSyntax { Condition: BinaryExpressionSyntax binaryCondition } &&
            (binaryCondition.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) ||
             binaryCondition.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken)) &&
            binaryCondition.Right.Kind() == SyntaxKind.NullLiteralExpression &&
            TryGetExpressionMemberName(binaryCondition.Left, out propertyName))
        {
            return true;
        }

        return false;
    }
}