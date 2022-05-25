using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace AutoMapper.Analyzers.Common.Tests;

public class AnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier> where TAnalyzer : DiagnosticAnalyzer, new()
{
    public AnalyzerTest()
    {
        ReferenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity(nameof(AutoMapper), "10.1.1")));
    }
}