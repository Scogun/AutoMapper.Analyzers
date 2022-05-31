using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Testing;

namespace AutoMapper.Analyzers.Common.Tests;

public class MapBaseAnalyzerTests<TAnalyzer> : BaseAnalyzerTests
    where TAnalyzer : ForMemberAnalyzer, new() 
{
    protected const string CreateMapCode = @"CreateMap<InputObject, OutputObject>{0}";

    private static string MapName => string.Format(CreateMapCode, string.Empty);

    protected async Task VerifyExpectedAsync(string diagnosticId, string forMembers, int locationCount = 1)
    {
        var badMapFrom = string.Format(CreateMapCode, forMembers);
        var expected = AnalyzerVerifier<TAnalyzer>.Diagnostic(diagnosticId).WithArguments("TestProfile", MapName);
        for (int i = 0; i < locationCount; i++)
        {
            expected = expected.WithLocation(i);
        }
        await VerifyAnalyzerAsync(badMapFrom, expected);
    }

    protected async Task VerifyAnalyzerAsync(string mapFrom, params DiagnosticResult[] expected)
    {
        await AnalyzerVerifier<TAnalyzer>.VerifyAnalyzerAsync(ClassSourceCode(mapFrom), expected);
    }
    
    protected async Task VerifyCodeFixAsync<TCodeFix>(string diagnosticId, string forMember, string forMemberFix, int locationCount = 1)
        where TCodeFix : CodeFixProvider, new()
    {
        var mapFrom = string.Format(CreateMapCode, forMember);
        var fixMapFrom = string.Format(CreateMapCode, forMemberFix);
        var expected = AnalyzerVerifier<TAnalyzer>.Diagnostic(diagnosticId).WithArguments("TestProfile", MapName);
        for (int i = 0; i < locationCount; i++)
        {
            expected = expected.WithLocation(i);
        }
        await AnalyzerVerifier<TAnalyzer>.VerifyCodeFixAsync<TCodeFix>(ClassSourceCode(mapFrom), expected, ClassSourceCode(fixMapFrom, true));
    }
}