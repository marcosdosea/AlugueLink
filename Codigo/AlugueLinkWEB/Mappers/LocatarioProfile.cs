using AutoMapper;
using AlugueLinkWEB.Models;
using Core;
using System.Text.RegularExpressions;

namespace AlugueLinkWEB.Mappers
{
    public class LocatarioProfile : Profile
    {
        public LocatarioProfile()
        {
            CreateMap<LocatarioViewModel, Locatario>()
                .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => LimparCep(src.Cep)))
                .ReverseMap()
                .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => FormatarCep(src.Cep)));
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