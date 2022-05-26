using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class NullSubstituteTests : MapBaseAnalyzerTests<NullSubstituteAnalyzer>
{
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => {|#0:opt.MapFrom(src => src.Id ?? 10)|});", TestName = "Null-coalescing operator")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => {|#0:opt.MapFrom(src => src.Id == null ? 10 : src.Id)|});", TestName = "Ternary conditional operator with equals")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => {|#0:opt.MapFrom(src => src.Id != null ? src.Id : 10)|});", TestName = "Ternary conditional operator with don't equals")]
    [TestCase("()\n\r.ForMember(dest => dest.Device, opt => {|#0:opt.MapFrom(src => src.Device != null ? src.Device.Name : null)|});", TestName = "Ternary conditional operator no flat structure")]
    public async Task NullSubstitutionAnalyzerRaiseDiagnostic(string forMembers)
    {
        await VerifyExpectedAsync(NullSubstituteAnalyzer.DiagnosticId, forMembers);
    }
}