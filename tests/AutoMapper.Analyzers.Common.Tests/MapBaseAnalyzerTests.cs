using Microsoft.CodeAnalysis.Testing;

namespace AutoMapper.Analyzers.Common.Tests;

public class MapBaseAnalyzerTests<TAnalyzer> : BaseAnalyzerTests
    where TAnalyzer : ForMemberAnalyzer, new() 
{
    protected const string CreateMapCode = @"CreateMap<InputObject, OutputObject>{0}";

    private static string MapName => string.Format(CreateMapCode, string.Empty);

    protected async Task VerifyExpectedAsync(string diagnosticId, string forMembers)
    {
        var badMapFrom = string.Format(CreateMapCode, forMembers);
        var expected = AnalyzerVerifier<TAnalyzer>.Diagnostic(diagnosticId).WithArguments("TestProfile", MapName).WithLocation(0);
        await VerifyAnalyzerAsync(badMapFrom, expected);
    }

    protected async Task VerifyAnalyzerAsync(string mapFrom, params DiagnosticResult[] expected)
    {
        await AnalyzerVerifier<TAnalyzer>.VerifyAnalyzerAsync(ClassSourceCode(mapFrom), expected);
    }
}