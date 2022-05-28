using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class ConstructorTests : BaseAnalyzerTests
{
    [TestCase("{|#0:public TestProfile() { }|}", TestName = "Empty Profile Constructor")]
    [TestCase("{|#0:public TestProfile() { Console.WriteLine(); }|}", TestName = "Profile Constructor with only extra invocation")]
    [TestCase("{|#0:public TestProfile() { if (true) {} }|}", TestName = "Profile Constructor with only extra statement")]
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

    [TestCase("CreateMap<InputObject, OutputObject>();", TestName = "CreateMap into constructor")]
    [TestCase("try { CreateMap<InputObject, OutputObject>(); } catch (Exception e) { Console.WriteLine(e.Message); }", TestName = "CreateMap into constructor into try-catch/finally should not raise AMA0001 diagnostic")]
    public async Task ConstructorAnalyzerSuccess(string constructorBody)
    {
        await AnalyzerVerifier<ConstructorAnalyzer>.VerifyAnalyzerAsync(ClassSourceCode(constructorBody));
    }
}
