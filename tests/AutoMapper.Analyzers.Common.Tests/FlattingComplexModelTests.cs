using NUnit.Framework;

namespace AutoMapper.Analyzers.Common.Tests;

public class FlattingComplexModelTests : MapBaseAnalyzerTests<FlattingComplexModelAnalyzer>
{
    [TestCase("()\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", 2, TestName = "Two mappings from complex model")]
    [TestCase("()\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name))\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", 2, TestName = "Two mappings from complex model with additional ForMember")]
    [TestCase("()\n\r.ForMember{|#2:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#1:(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", 3, TestName = "Three mappings from complex model")]
    public async Task FlatteningComplexModelAnalyzerRaiseDiagnostic(string forMembers, int locationCount)
    {
        await VerifyExpectedAsync(FlattingComplexModelAnalyzer.DiagnosticId, forMembers, locationCount);
    }
    
    [TestCase("()\n\r.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));", TestName = "Usual flattening should not be diagnosed")]
    [TestCase("()\n\r.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));", TestName = "Only one complex flattening should not be diagnosed")]
    [TestCase("()\n\r.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));", TestName = "Similar properties names should not be diagnosed")]
    public async Task FlatteningComplexModelAnalyzerSuccess(string forMembers)
    {
        var goodMapFrom = string.Format(CreateMapCode, forMembers);
        await VerifyAnalyzerAsync(goodMapFrom);
    }

    [TestCase("()\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", "()\n\r.IncludeMembers(src => src.User);", 2, TestName = "Two mappings from complex model should be replaced by IncludeMembers call")]
    [TestCase("()\n\r.IncludeMembers(source => source.Device)\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", "()\n\r.IncludeMembers(source => source.Device, src => src.User);", 2, TestName = "Two mappings from complex model should be added into available IncludeMembers call")]
    [TestCase("()\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name))\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", "()\n\r.IncludeMembers(src => src.User)\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name));", 2, TestName = "Two mappings from complex model with additional ForMember before should be replaced by IncludeMembers call")]
    [TestCase("()\n\r.IncludeMembers(src => src.Device)\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name))\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", "()\n\r.IncludeMembers(src => src.Device, src => src.User)\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name));", 2, TestName = "Two mappings from complex model with additional ForMember before should be added into available at first place IncludeMembers call")]
    [TestCase("()\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name))\n\r.IncludeMembers(src => src.Device)\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", "().ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name))\n\r.IncludeMembers(src => src.Device, src => src.User);", 2, TestName = "Two mappings from complex model with additional ForMember before should be added into available at last place IncludeMembers call")]
    [TestCase("()\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|}\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name));", "()\n\r.IncludeMembers(src => src.User)\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name));", 2, TestName = "Two mappings from complex model with additional ForMember after should be replaced by IncludeMembers call")]
    [TestCase("()\n\r.ForMember{|#1:(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))|}\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name))\n\r.ForMember{|#0:(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))|};", "()\n\r.IncludeMembers(src => src.User)\n\r.ForMember(dest => dest.Device, opt => opt.MapFrom(src => src.Device.Name));", 2, TestName = "Two mappings from complex model with additional ForMember between should be replaced by IncludeMembers call")]
    public async Task FlatteningComplexModelFixCheck(string forMembers, string includedMember, int locationCount)
    {
        await VerifyCodeFixAsync<FlattingComplexModelCodeFixProvider>(FlattingComplexModelAnalyzer.DiagnosticId, forMembers, includedMember, locationCount);
    }
}