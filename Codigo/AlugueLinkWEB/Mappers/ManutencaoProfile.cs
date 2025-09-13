using AutoMapper;
using Core.DTO;
using Core;

namespace AlugueLinkWEB.Mappers
{
    public class ManutencaoProfile : Profile
    {
        public ManutencaoProfile()
        {
            CreateMap<ManutencaoDto, Manutencao>()
                .ReverseMap();
        }
    }
}