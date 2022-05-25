using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace AutoMapper.Analyzers.Common.Tests;

public static class AnalyzerVerifier<TAnalyzer> where TAnalyzer: DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, NUnitVerifier>.Diagnostic(diagnosticId);

    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new AnalyzerTest<TAnalyzer>
        {
            TestCode = source
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }
    
    public static async Task VerifyCodeFixAsync<TCodeFix>(string source, DiagnosticResult expected, string fixedSource)
        where TCodeFix : CodeFixProvider, new()
        => await VerifyCodeFixAsync<TCodeFix>(source, new[] { expected }, fixedSource);
    
    public static async Task VerifyCodeFixAsync<TCodeFix>(string source, DiagnosticResult[] expected, string fixedSource)
        where TCodeFix : CodeFixProvider, new()
    {
        var test = new CodeFixTest<TAnalyzer, TCodeFix>
        {
            TestCode = source,
            FixedCode = fixedSource
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }
}