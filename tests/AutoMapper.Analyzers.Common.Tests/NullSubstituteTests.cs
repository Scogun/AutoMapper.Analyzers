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
    
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => {|#0:opt.MapFrom(src => src.Id ?? 10)|});", "()\n\r.ForMember(dest => dest.Id, opt => opt.NullSubstitute(10));", TestName = "Fix null-coalescing operator, opt lambda parameter")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, options => {|#0:options.MapFrom(src => src.Id ?? 10)|});", "()\n\r.ForMember(dest => dest.Id, options => options.NullSubstitute(10));", TestName = "Fix null-coalescing operator, options lambda parameter")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => {|#0:opt.MapFrom(src => src.Id == null ? 10 : src.Id)|});", "()\n\r.ForMember(dest => dest.Id, opt => opt.NullSubstitute(10));", TestName = "Fix ternary conditional operator with equals")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => {|#0:opt.MapFrom(src => src.Id != null ? src.Id : 10)|});", "()\n\r.ForMember(dest => dest.Id, opt => opt.NullSubstitute(10));", TestName = "Fix ternary conditional operator with don't equals")]
    [TestCase("()\n\r.ForMember(dest => dest.Device, opt => {|#0:opt.MapFrom(src => src.Device != null ? src.Device.Name : null)|});", "()\n\r.ForMember(dest => dest.Device, opt => { opt.NullSubstitute(null); opt.MapFrom(src => src.Device.Name); });", TestName = "Fix ternary conditional operator no flat structure with opt lambda parameter")]
    [TestCase("()\n\r.ForMember(dest => dest.Device, options => {|#0:options.MapFrom(source => source.Device != null ? source.Device.Name : null)|});", "()\n\r.ForMember(dest => dest.Device, options => { options.NullSubstitute(null); options.MapFrom(source => source.Device.Name); });", TestName = "Fix ternary conditional operator no flat structure with options lambda parameter")]
    public async Task NullSubstitutionFixChecking(string forMembers, string nullSubstitute)
    {
        await VerifyCodeFixAsync<NullSubstituteCodeFixProvider>(NullSubstituteAnalyzer.DiagnosticId, forMembers, nullSubstitute);
    }
}