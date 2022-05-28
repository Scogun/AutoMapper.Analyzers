using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class FlatteningTests : MapBaseAnalyzerTests<FlatteningAnalyzer>
{
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))|};", TestName = "Diagnostic should be raised")]
    public async Task FlatteningAnalyzerRaiseDiagnostic(string forMembers)
    {
        await VerifyExpectedAsync(FlatteningAnalyzer.DiagnosticId, forMembers);
    }

    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));", TestName = "Diagnostic should not be raised")]
    public async Task FlatteningAnalyzerSuccess(string forMembers)
    {
        var goodMapFrom = string.Format(CreateMapCode, forMembers);
        await VerifyAnalyzerAsync(goodMapFrom);
    }

    [TestCase("()\n\r.ForMember{|#0:(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))|};", "()\n;", TestName = "Only flatting map should be removed")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n\r.ForMember{|#0:(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))|};", "()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore())\n;", TestName = "Flatting map should be removed, other map before")]
    [TestCase("()\n\r.ForMember{|#0:(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember(dest => dest.Name, opt => opt.Ignore());", "()\n\r.ForMember(dest => dest.Name, opt => opt.Ignore());", TestName = "Flatting map should be removed, other map after")]
    public async Task FlatteningAnalyzerFixChecking(string forMembers, string forMemberFix)
    {
        await VerifyCodeFixAsync<ForMemberCodeFixProvider>(FlatteningAnalyzer.DiagnosticId, forMembers, forMemberFix);
    }
}