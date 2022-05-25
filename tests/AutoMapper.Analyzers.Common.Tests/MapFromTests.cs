using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class MapFromTests : MapBaseAnalyzerTests<MapFromAnalyzer>
{
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};")]
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|}\n\r.ForMember(dest => dest.UserName, opt => opt.Ignore());")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};")]
    public async Task MapFromAnalyzerRaiseDiagnostic(string forMembers)
    {
        await VerifyExpectedAsync(MapFromAnalyzer.DiagnosticId, forMembers);
    }

    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomId));")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 10));")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 10));")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(new IdResolver()));")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom<IdResolver>());")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))\n\r.ForAllOtherMembers(opt => opt.Ignore());")]
    public async Task MapFromAnalyzerSuccess(string forMembers)
    {
        var goodMapFrom = string.Format(CreateMapCode, forMembers);
        await VerifyAnalyzerAsync(goodMapFrom);
    }
    
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};", "()\n;")]
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|}\n\r.ForMember(dest => dest.UserName, opt => opt.Ignore());", "()\n\r.ForMember(dest => dest.UserName, opt => opt.Ignore());")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};", "()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n;")]
    public async Task MapFromAnalyzerFixChecking(string forMembers, string forMemberFix)
    {
        await VerifyCodeFixAsync<ForMemberCodeFixProvider>(MapFromAnalyzer.DiagnosticId, forMembers, forMemberFix);
    }
}