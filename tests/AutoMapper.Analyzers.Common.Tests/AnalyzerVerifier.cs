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
}