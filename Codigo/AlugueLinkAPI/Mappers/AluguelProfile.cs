using AutoMapper;
using Core;
using Models;

namespace Mappers
{
    public class AluguelProfile : Profile
    {
        public AluguelProfile()
        {
            CreateMap<AluguelViewModel, Aluguel>().ReverseMap();
        }
    }
}