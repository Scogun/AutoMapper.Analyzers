using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class ConstructorTests : BaseAnalyzerTests
{
    [TestCase("{|#0:public TestProfile() { }|}")]
    [TestCase("{|#0:public TestProfile() { Console.WriteLine(); }|}")]
    [TestCase("{|#0:public TestProfile() { if (true) {} }|}")]
    public async Task ConstructorAnalyzerRaiseDiagnostic(string constructorBody)
    {
        var expected = AnalyzerVerifier<ConstructorAnalyzer>.Diagnostic(ConstructorAnalyzer.DiagnosticId).WithArguments("TestProfile").WithLocation(0);
        await AnalyzerVerifier<ConstructorAnalyzer>.VerifyAnalyzerAsync(EmptyProfileSourceCode(constructorBody), expected);
    }
    
    [Test]
    public async Task NoConstructorAnalyzerRaiseDiagnostic()
    {
        var expected = AnalyzerVerifier<ConstructorAnalyzer>.Diagnostic(ConstructorAnalyzer.DiagnosticId).WithArguments("TestProfile").WithLocation(6, 5);
        await AnalyzerVerifier<ConstructorAnalyzer>.VerifyAnalyzerAsync(EmptyProfileSourceCode(string.Empty), expected);
    }
}