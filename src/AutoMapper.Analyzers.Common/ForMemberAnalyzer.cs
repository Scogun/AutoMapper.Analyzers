using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace AutoMapper.Analyzers.Common;

public abstract class ForMemberAnalyzer : BaseAnalyzer
{
    public override DiagnosticCategory Category => DiagnosticCategory.ForMemberCategory;
    
    protected InvocationExpressionSyntax ForMember { get; private set; }

    protected string MapName { get; private set; }
    
    protected override Diagnostic AnalyzeInvocationOperation(IInvocationOperation invocationOperation)
    {
        if (invocationOperation.TargetMethod.Name == nameof(IMappingExpression.ForMember) && invocationOperation.Syntax is InvocationExpressionSyntax forMember)
        {
            ForMember = forMember;
            MapName = forMember.DescendantNodes().OfType<GenericNameSyntax>().ToList()[0].ToString();
            var expressions = GetLambdaExpressions(forMember);
            if (expressions.descExpression != null)
            {
                return AnalyzeMapFrom(expressions.descExpression, expressions.srcExpression);
            }
        }

        return base.AnalyzeInvocationOperation(invocationOperation);
    }
    
    protected (LambdaExpressionSyntax descExpression, LambdaExpressionSyntax srcExpression) GetLambdaExpressions(InvocationExpressionSyntax forMember)
    {
        var destExpression = forMember.ArgumentList.Arguments[0].Expression as LambdaExpressionSyntax;
        var optExpression = forMember.ArgumentList.Arguments[1].Expression as LambdaExpressionSyntax;
        if (TryGetExpressionMemberName(optExpression, out string optName) && optName.Equals(nameof(IMemberConfigurationExpression.MapFrom)))
        {
            if ((optExpression.ExpressionBody as InvocationExpressionSyntax).ArgumentList.Arguments[0].Expression is LambdaExpressionSyntax srcExpression)
            {
                return (destExpression, srcExpression);
            }
        }

        return default;
    }
    
    protected virtual Diagnostic AnalyzeMapFrom(LambdaExpressionSyntax destExpression, LambdaExpressionSyntax srcExpression)
    {
        return default;
    }
    
    protected static bool TryGetExpressionMemberName(LambdaExpressionSyntax syntax, out IdentifierNameSyntax name)
    {
        if (syntax?.ExpressionBody is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax expressionMemberAccess } && TryGetExpressionMemberName(expressionMemberAccess, out name))
        {
            return true;
        }

        return TryGetExpressionMemberName(syntax?.ExpressionBody, out name);
    }
    
    protected static bool TryGetExpressionMemberName(LambdaExpressionSyntax syntax, out string name)
    {
        name = null;
        if (TryGetExpressionMemberName(syntax, out IdentifierNameSyntax memberName))
        {
            name = memberName.ToString();
            return true;
        }

        return false;
    }
    
    protected static bool TryGetExpressionMemberName(ExpressionSyntax syntax, out IdentifierNameSyntax name)
    {
        name = null;
        if (syntax is MemberAccessExpressionSyntax {Name: IdentifierNameSyntax identifierName})
        {
            name = identifierName;
            return true;
        }

        return false;
    }
}