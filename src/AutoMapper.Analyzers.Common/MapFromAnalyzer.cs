using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoMapper.Analyzers.Common;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MapFromAnalyzer : ForMemberAnalyzer
{
    public const string DiagnosticId = "AMA0002";

    protected override string InternalDiagnosticId => DiagnosticId;
    
    protected override Diagnostic AnalyzeMapFrom(LambdaExpressionSyntax destExpression, LambdaExpressionSyntax srcExpression)
    {
        if (TryGetExpressionMemberName(srcExpression, out IdentifierNameSyntax srcMember) && TryGetExpressionMemberName(destExpression, out string destMember))
        {
            var fullSrcMember = srcMember.Parent.ToFullString();
            var chainMemberCalls = fullSrcMember.Substring(fullSrcMember.IndexOf('.') + 1);
            if (chainMemberCalls.Equals(destMember) && !MapHasAllIgnore)
            {
                return Diagnostic.Create(Rule, ForMember.ArgumentList.GetLocation(), ProfileName, MapName);
            }
        }

        return base.AnalyzeMapFrom(destExpression, srcExpression);
    }
    
    private bool MapHasAllIgnore {
        get
        {
            var parentStatement = ParentExpressionStatement;
            var lastMemberExpression = parentStatement.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
            if (!lastMemberExpression.Name.ToString().Equals(nameof(IMappingExpression.ForAllOtherMembers), StringComparison.Ordinal))
            {
                return false;
            }

            var optExpression = (lastMemberExpression.Parent as InvocationExpressionSyntax).ArgumentList.Arguments[0].Expression as LambdaExpressionSyntax;
            return TryGetExpressionMemberName(optExpression, out string optName) && optName.Equals(nameof(IMemberConfigurationExpression.Ignore), StringComparison.Ordinal);
        }
    }
    
    private ExpressionStatementSyntax ParentExpressionStatement
    {
        get
        {
            var parent = ForMember.Parent;
            while (parent != null && !(parent is ExpressionStatementSyntax))
            {
                parent = parent.Parent;
            }

            return parent as ExpressionStatementSyntax;
        }
    }
}