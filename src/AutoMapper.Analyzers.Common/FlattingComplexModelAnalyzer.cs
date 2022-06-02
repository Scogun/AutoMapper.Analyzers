using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FlattingComplexModelAnalyzer : ForMemberAnalyzer
{
    public const string DiagnosticId = "AMA0005";

    protected override string InternalDiagnosticId => DiagnosticId;
    
    protected override Diagnostic AnalyzeMapFrom(LambdaExpressionSyntax destExpression,
            LambdaExpressionSyntax srcExpression)
        {
            if (TryGetExpressionMemberName(srcExpression, out IdentifierNameSyntax srcName) &&
                TryGetExpressionMemberName(destExpression, out string destName) &&
                srcName.ToString().Equals(destName) && srcName.Parent is MemberAccessExpressionSyntax srcMember)
            {
                var srcMemberCall = srcMember.Expression.ToFullString();
                var nextMembers = ForMember.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(IsForMember).ToList();
                var prevMembers = ForMember.Ancestors().OfType<InvocationExpressionSyntax>().Where(IsForMember);
                if (!prevMembers.Any(s => IsSameComplexFlatting(s, srcMemberCall)) &&
                    nextMembers.Any(s => IsSameComplexFlatting(s, srcMemberCall)))
                {
                    return Diagnostic.Create(Rule, ForMember.ArgumentList.GetLocation(), nextMembers.Select(m => m.ArgumentList.GetLocation()).ToList(),ProfileName, MapName);
                }
            }

            return base.AnalyzeMapFrom(destExpression, srcExpression);
        }

        private bool IsSameComplexFlatting(InvocationExpressionSyntax syntax, string smelledMemberCall) =>
            TryGetExpressionMemberName(GetLambdaExpressions(syntax).srcExpression,
                out IdentifierNameSyntax srcSyntax)
            && srcSyntax.Parent is MemberAccessExpressionSyntax srcAccess &&
            srcAccess.Expression.ToFullString().Equals(smelledMemberCall);

        private bool IsForMember(InvocationExpressionSyntax syntax)
        {
            if (syntax.Expression is MemberAccessExpressionSyntax memberAccess && memberAccess.Name.Identifier.Text.Equals(nameof(IMappingExpression.ForMember)))
            {
                var (descExpression, srcExpression) = GetLambdaExpressions(syntax);
                return TryGetExpressionMemberName(descExpression, out string destName) &&
                       TryGetExpressionMemberName(srcExpression, out string srcName)
                       && destName.Equals(srcName);
            }

            return false;
        }
}