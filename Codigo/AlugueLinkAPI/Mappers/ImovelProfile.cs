using AutoMapper;
using Core;
using Models;

namespace Mappers
{
    public class ImovelProfile : Profile
    {
        public ImovelProfile()
        {
            CreateMap<ImovelViewModel, Imovel>().ReverseMap();
        }
    }
}