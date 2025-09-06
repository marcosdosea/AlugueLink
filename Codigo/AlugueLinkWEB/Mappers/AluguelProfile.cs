using AutoMapper;
using Core.DTO;
using Core;

namespace AlugueLinkWEB.Mappers
{
    /// <summary>
    /// Perfil AutoMapper para Aluguel
    /// </summary>
    public class AluguelProfile : Profile
    {
        public AluguelProfile()
        {
            CreateMap<AluguelDTO, Aluguel>()
                .ReverseMap();
        }
    }
}