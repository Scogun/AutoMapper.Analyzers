using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FlatteningAnalyzer : ForMemberAnalyzer
{
    public const string DiagnosticId = "AMA0006";

    protected override string InternalDiagnosticId => DiagnosticId;
    
    protected override Diagnostic AnalyzeMapFrom(LambdaExpressionSyntax destExpression, LambdaExpressionSyntax srcExpression)
    {
        if (TryGetExpressionMemberName(srcExpression, out IdentifierNameSyntax srcMember) && TryGetExpressionMemberName(destExpression, out string destMember))
        {
            var fullSrcMember = srcMember.Parent.ToFullString();
            var chainMemberCallsJoin = fullSrcMember.Substring(fullSrcMember.IndexOf('.') + 1);
            if (chainMemberCallsJoin.Contains(".") && chainMemberCallsJoin.Replace(".", "").Equals(destMember))
            {
                return Diagnostic.Create(Rule, ForMember.ArgumentList.GetLocation(), ProfileName, MapName);
            }
        }

        return base.AnalyzeMapFrom(destExpression, srcExpression);
    }
}