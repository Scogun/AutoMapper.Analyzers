using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace AutoMapper.Analyzers.Common;

public abstract class CreateMapAnalyzer : BaseAnalyzer
{
    public override DiagnosticCategory Category => DiagnosticCategory.CreateMapCategory;

    protected override Diagnostic AnalyzeInvocationOperation(IInvocationOperation invocationOperation)
    {
        if (invocationOperation.TargetMethod.Name == nameof(Profile.CreateMap) && invocationOperation.Syntax is InvocationExpressionSyntax createMap)
        {
            return AnalyzeCreateMap(createMap);
        }

        return base.AnalyzeInvocationOperation(invocationOperation);
    }
    
    protected virtual Diagnostic AnalyzeCreateMap(InvocationExpressionSyntax createMapSyntax)
    {
        return default;
    }
}