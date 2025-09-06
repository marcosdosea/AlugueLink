using AutoMapper;
using Core.DTO;
using Core;

namespace AlugueLinkWEB.Mappers
{
    /// <summary>
    /// Perfil AutoMapper para Manutencao
    /// </summary>
    public class ManutencaoProfile : Profile
    {
        public ManutencaoProfile()
        {
            CreateMap<ManutencaoDTO, Manutencao>()
                .ReverseMap();
        }
    }
}