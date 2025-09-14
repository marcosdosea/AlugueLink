using AutoMapper;
using Core;
using Models;

namespace Mappers
{
    public class LocadorProfile : Profile
    {
        public LocadorProfile()
        {
            CreateMap<LocadorViewModel, Locador>().ReverseMap();
        }
    }
}