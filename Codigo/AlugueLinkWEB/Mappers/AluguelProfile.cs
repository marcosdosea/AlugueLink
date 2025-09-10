using AutoMapper;
using Core.DTO;
using Core;
using AlugueLinkWEB.Models;

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

            CreateMap<AluguelViewModel, Aluguel>()
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio ?? DateOnly.MinValue))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim ?? DateOnly.MinValue))
                .ForMember(dest => dest.DataAssinatura, opt => opt.MapFrom(src => src.DataAssinatura ?? DateOnly.MinValue))
                .ForMember(dest => dest.Idlocatario, opt => opt.MapFrom(src => src.IdLocatario ?? 0))
                .ForMember(dest => dest.Idimovel, opt => opt.MapFrom(src => src.IdImovel ?? 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapStatusToDatabase(src.Status)));

            CreateMap<Aluguel, AluguelViewModel>()
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => (DateOnly?)src.DataInicio))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => (DateOnly?)src.DataFim))
                .ForMember(dest => dest.DataAssinatura, opt => opt.MapFrom(src => (DateOnly?)src.DataAssinatura))
                .ForMember(dest => dest.IdLocatario, opt => opt.MapFrom(src => (int?)src.Idlocatario))
                .ForMember(dest => dest.IdImovel, opt => opt.MapFrom(src => (int?)src.Idimovel))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // Usar código direto do banco (A, F, P)
                .ForMember(dest => dest.LocatarioNome, opt => opt.MapFrom(src => src.IdlocatarioNavigation != null ? src.IdlocatarioNavigation.Nome : ""))
                .ForMember(dest => dest.ImovelEndereco, opt => opt.MapFrom(src => src.IdimovelNavigation != null ? 
                    $"{src.IdimovelNavigation.Logradouro}, {src.IdimovelNavigation.Numero} - {src.IdimovelNavigation.Bairro}" : ""))
                .ForMember(dest => dest.ImovelTipo, opt => opt.MapFrom(src => src.IdimovelNavigation != null ? src.IdimovelNavigation.Tipo : ""))
                .ForMember(dest => dest.ImovelValor, opt => opt.MapFrom(src => src.IdimovelNavigation != null ? (decimal?)src.IdimovelNavigation.Valor : null));
        }

        private static string? MapStatusFromDatabase(string? statusDb)
        {
            return statusDb switch
            {
                "A" => "Ativo",
                "F" => "Finalizado",
                "P" => "Pendente",
                _ => statusDb
            };
        }

        private static string? MapStatusToDatabase(string? statusView)
        {
            return statusView switch
            {
                "Ativo" => "A",
                "Finalizado" => "F", 
                "Pendente" => "P",
                "A" => "A", // Já está no formato correto
                "F" => "F", // Já está no formato correto
                "P" => "P", // Já está no formato correto
                _ => statusView ?? "A" // Default para Ativo se não especificado
            };
        }
    }
}