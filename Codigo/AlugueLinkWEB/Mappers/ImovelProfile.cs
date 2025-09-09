using AutoMapper;
using AlugueLinkWEB.Models;
using Core;

namespace AlugueLinkWEB.Mappers
{
    /// <summary>
    /// Perfil AutoMapper para Imovel
    /// </summary>
    public class ImovelProfile : Profile
    {
        public ImovelProfile()
        {
            CreateMap<ImovelViewModel, Imovel>()
                // Não forçar mais IdLocador = 1; deixar 0 para o serviço tratar se vier nulo
                .ForMember(dest => dest.IdLocador, opt => opt.MapFrom(src => src.LocadorId ?? 0))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => MapTipoToDatabase(src.Tipo)))
                .ReverseMap()
                .ForMember(dest => dest.LocadorId, opt => opt.MapFrom(src => (int?)src.IdLocador))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => MapTipoFromDatabase(src.Tipo)));
        }

        private static string? MapTipoFromDatabase(string? tipoDb)
        {
            return tipoDb switch
            {
                "C" => "casa",
                "A" => "apartamento",
                "PC" => "comercial",
                _ => tipoDb
            };
        }

        private static string? MapTipoToDatabase(string? tipoView)
        {
            return tipoView?.ToLower() switch
            {
                "casa" => "C",
                "apartamento" => "A",
                "comercial" => "PC",
                _ => tipoView
            };
        }
    }
}