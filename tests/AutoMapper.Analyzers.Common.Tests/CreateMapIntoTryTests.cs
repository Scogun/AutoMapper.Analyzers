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
    
    [TestCase("try { {|#0:CreateMap<InputObject, OutputObject>();|} } catch (Exception e) { Console.WriteLine(e.Message); }", "CreateMap<InputObject, OutputObject>(); try { } catch (Exception e) { Console.WriteLine(e.Message); }", TestName = "Extract CreateMap from empty try")]
    [TestCase("try { {|#0:CreateMap<InputObject, OutputObject>().ForMember(dest => dest.Device, opt => opt.Ignore());|} } catch (Exception e) { Console.WriteLine(e.Message); }", "CreateMap<InputObject, OutputObject>().ForMember(dest => dest.Device, opt => opt.Ignore()); try { } catch (Exception e) { Console.WriteLine(e.Message); }", TestName = "Extract CreateMap with ForMember from empty try")]
    [TestCase("try { {|#0:CreateMap<InputObject, OutputObject>();|} Console.WriteLine(); } catch (Exception e) { Console.WriteLine(e.Message); }", "CreateMap<InputObject, OutputObject>(); try { Console.WriteLine(); } catch (Exception e) { Console.WriteLine(e.Message); }", TestName = "Extract CreateMap from try with additional invocations")]
    public async Task Fix(string constructorBody, string fixedConstructorBody)
    {
        var expected = AnalyzerVerifier<CreateMapIntoTryAnalyzer>.Diagnostic(CreateMapIntoTryAnalyzer.DiagnosticId).WithArguments("TestProfile", "CreateMap<InputObject, OutputObject>", "try-catch").WithLocation(0);
        await AnalyzerVerifier<CreateMapIntoTryAnalyzer>.VerifyCodeFixAsync<CreateMapIntoTryCodeFixProvider>(ClassSourceCode(constructorBody), expected, ClassSourceCode(fixedConstructorBody, true));
    }
}