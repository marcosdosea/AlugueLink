using AutoMapper;
using AlugueLinkWEB.Models;
using Core.DTO;
using Core;

namespace AlugueLinkWEB.Mappers
{
    /// <summary>
    /// Perfil AutoMapper para Locador
    /// </summary>
    public class LocadorProfile : Profile
    {
        public LocadorProfile()
        {
            CreateMap<LocadorDto, Locador>()
                .ReverseMap();
            
            CreateMap<LocadorViewModel, Locador>()
                .ReverseMap();
        }
    }
}