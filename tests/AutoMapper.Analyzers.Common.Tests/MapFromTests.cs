using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class MapFromTests : MapBaseAnalyzerTests<MapFromAnalyzer>
{
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};", TestName = "Only identical properties map")]
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|}\n\r.ForMember(dest => dest.UserName, opt => opt.Ignore());", TestName = "Identical properties map with Ignore after")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};", TestName = "Identical properties map with Ignore before")]
    public async Task MapFromAnalyzerRaiseDiagnostic(string forMembers)
    {
        await VerifyExpectedAsync(MapFromAnalyzer.DiagnosticId, forMembers);
    }

    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomId));", TestName = "Different properties map")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));", TestName = "Internal object identical properties map")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 10));", TestName = "Identical properties map with manually NullSubstitute")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 10));", TestName = "Constant map")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(new IdResolver()));", TestName = "Map by new Resolver instance")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom<IdResolver>());", TestName = "Map by generic Resolver")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))\n\r.ForAllOtherMembers(opt => opt.Ignore());", TestName = "Identical properties map with ForAllOtherMembers ignore")]
    public async Task MapFromAnalyzerSuccess(string forMembers)
    {
        var goodMapFrom = string.Format(CreateMapCode, forMembers);
        await VerifyAnalyzerAsync(goodMapFrom);
    }
    
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};", "()\n;", TestName = "Fix identical properties map when it is only it")]
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|}\n\r.ForMember(dest => dest.UserName, opt => opt.Ignore());", "()\n\r.ForMember(dest => dest.UserName, opt => opt.Ignore());", TestName = "Fix identical properties map with Ignore after")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n\r.ForMember{|#0:(dest => dest.Id, opt => opt.MapFrom(src => src.Id))|};", "()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n;", TestName = "Fix identical properties map with Ignore before")]
    public async Task MapFromAnalyzerFixChecking(string forMembers, string forMemberFix)
    {
        await VerifyCodeFixAsync<ForMemberCodeFixProvider>(MapFromAnalyzer.DiagnosticId, forMembers, forMemberFix);
    }
}