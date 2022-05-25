using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace AutoMapper.Analyzers.Common.Tests;

public class CodeFixTest<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, NUnitVerifier> 
    where TAnalyzer : DiagnosticAnalyzer, new() 
    where TCodeFix : CodeFixProvider, new()
{
    public CodeFixTest()
    {
        ReferenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity(nameof(AutoMapper), "10.1.1")));
    }
}