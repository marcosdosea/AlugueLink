using AutoMapper;
using Core;
using Models;

namespace Mappers
{
    public class LocatarioProfile : Profile
    {
        public LocatarioProfile()
        {
            CreateMap<LocatarioViewModel, Locatario>().ReverseMap();
        }
    }
}