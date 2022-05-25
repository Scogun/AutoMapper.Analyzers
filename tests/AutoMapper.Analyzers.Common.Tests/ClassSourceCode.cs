using System;
using AutoMapper;

namespace AutoMapper.Analyzer.Common.Tests
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            //%#Replace#%
        }
    }

    public class InputObject
    {
        public int? Id { get; set; }

        public int CustomId { get; set; }

        public Device Device { get; set; }

        public User User { get; set; }
    }

    public class Device
    {
        public string Name { get; set; }
    }

    public class User
    {
        public string Name { get; set; }

        public string Surname { get; set; }
    }

    public class OutputObject
    {
        public int Id { get; set; }

        public string Device { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }
    }

    public class IdResolver : IValueResolver<InputObject, OutputObject, int>
    {
        public int Resolve(InputObject source, OutputObject destination, int destMember, ResolutionContext context)
        {
            return source.Id ?? source.CustomId;
        }
    }
}