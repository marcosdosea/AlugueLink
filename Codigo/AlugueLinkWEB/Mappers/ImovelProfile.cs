using AutoMapper;
using AlugueLinkWEB.Models;
using Core;
using System.Text.RegularExpressions;
using System.Globalization;

namespace AlugueLinkWEB.Mappers
{
    public class ImovelProfile : Profile
    {
        public ImovelProfile()
        {
            CreateMap<ImovelViewModel, Imovel>()
                .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => LimparCep(src.Cep)))
                .ForMember(dest => dest.IdLocador, opt => opt.MapFrom(src => src.LocadorId ?? 0))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => MapTipoToDatabase(src.Tipo)))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area ?? 0m))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor ?? 0m))
                .ReverseMap()
                .ForMember(dest => dest.LocadorId, opt => opt.MapFrom(src => (int?)src.IdLocador))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => MapTipoFromDatabase(src.Tipo)))
                .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => FormatarCep(src.Cep)))
                .ForMember(dest => dest.AreaStr, opt => opt.MapFrom(src => src.Area.ToString("0.##", new CultureInfo("pt-BR")).Replace('.', ',')))
                .ForMember(dest => dest.ValorStr, opt => opt.MapFrom(src => src.Valor.ToString("0.##", new CultureInfo("pt-BR")).Replace('.', ',')));
        }

        private static string? MapTipoFromDatabase(string? tipoDb)
        {
            return tipoDb switch
            {
                "C" => "casa",
                "A" => "apartamento",
                "PC" => "comercial",
                "COM" => "comercial",
                _ => tipoDb?.ToLower()
            };
        }

        private static string? MapTipoToDatabase(string? tipoView)
        {
            return tipoView?.ToLower() switch
            {
                "casa" => "C",
                "apartamento" => "A",
                "comercial" => "COM",
                _ => tipoView
            };
        }

        private static string? LimparCep(string? cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return cep;

            return Regex.Replace(cep, @"\D", "");
        }

        private static string? FormatarCep(string? cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return cep;

            var cepLimpo = Regex.Replace(cep, @"\D", "");
            if (cepLimpo.Length == 8)
            {
                return $"{cepLimpo.Substring(0, 5)}-{cepLimpo.Substring(5, 3)}";
            }

            return cep;
        }
    }
}