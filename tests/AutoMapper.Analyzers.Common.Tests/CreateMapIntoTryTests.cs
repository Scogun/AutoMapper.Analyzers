using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class CreateMapIntoTryTests : BaseAnalyzerTests
{
    [TestCase("try { {|#0:CreateMap<InputObject, OutputObject>();|} } catch (Exception e) { Console.WriteLine(e.Message); }", "try-catch", TestName = "CreateMap into try-catch")]
    [TestCase("try { {|#0:CreateMap<InputObject, OutputObject>().ForMember(dest => dest.Device, opt => opt.Ignore());|} } catch (Exception e) { Console.WriteLine(e.Message); }", "try-catch", TestName = "CreateMap with ForMember into try-catch")]
    [TestCase("try { {|#0:CreateMap<InputObject, OutputObject>().ForMember(dest => dest.Device, opt => opt.Ignore());|} } catch (Exception e) { Console.WriteLine(e.Message); } finally { Console.WriteLine(); }", "try-catch/finally", TestName = "CreateMap with ForMember into try-catch/finally")]
    public async Task Analyze(string constructorBody, string tryCatchFinally)
    {
        var expected = AnalyzerVerifier<CreateMapIntoTryAnalyzer>.Diagnostic(CreateMapIntoTryAnalyzer.DiagnosticId).WithArguments("TestProfile", "CreateMap<InputObject, OutputObject>", tryCatchFinally).WithLocation(0);
        await AnalyzerVerifier<CreateMapIntoTryAnalyzer>.VerifyAnalyzerAsync(ClassSourceCode(constructorBody), expected);
    }
}