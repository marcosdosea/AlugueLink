using AutoMapper;
using AlugueLinkWEB.Models;
using Core;

namespace AlugueLinkWEB.Mappers
{
    /// <summary>
    /// Perfil AutoMapper para Locatario
    /// </summary>
    public class LocatarioProfile : Profile
    {
        public LocatarioProfile()
        {
            CreateMap<LocatarioViewModel, Locatario>().ReverseMap();
        }
    }
}