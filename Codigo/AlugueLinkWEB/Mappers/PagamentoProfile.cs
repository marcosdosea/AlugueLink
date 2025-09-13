using AutoMapper;
using Core.DTO;
using Core;
using AlugueLinkWEB.Models;

namespace AlugueLinkWEB.Mappers
{
    public class PagamentoProfile : Profile
    {
        public PagamentoProfile()
        {
            CreateMap<PagamentoDto, Pagamento>()
                .ReverseMap();

            CreateMap<Pagamento, PagamentoViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.ValorStr, opt => opt.MapFrom(src => src.Valor.ToString("F2")))
                .ForMember(dest => dest.TipoPagamento, opt => opt.MapFrom(src => src.TipoPagamento))
                .ForMember(dest => dest.AluguelId, opt => opt.MapFrom(src => src.Idaluguel))
                .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.DataPagamento != default(DateTime) ? DateOnly.FromDateTime(src.DataPagamento) : (DateOnly?)null))
                .ForMember(dest => dest.HoraPagamento, opt => opt.MapFrom(src => src.DataPagamento != default(DateTime) ? TimeOnly.FromDateTime(src.DataPagamento) : (TimeOnly?)null))
                .ForMember(dest => dest.DataHoraPagamento, opt => opt.MapFrom(src => src.DataPagamento))
                
                .ForMember(dest => dest.ImovelEndereco, opt => opt.Ignore())
                .ForMember(dest => dest.LocatarioNome, opt => opt.Ignore())
                .ForMember(dest => dest.ValorAluguel, opt => opt.Ignore())
                .ForMember(dest => dest.DataInicioAluguel, opt => opt.Ignore())
                .ForMember(dest => dest.DataFimAluguel, opt => opt.Ignore())
                .ForMember(dest => dest.StatusAluguel, opt => opt.Ignore());

            CreateMap<PagamentoViewModel, Pagamento>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor ?? 0))
                .ForMember(dest => dest.TipoPagamento, opt => opt.MapFrom(src => src.TipoPagamento))
                .ForMember(dest => dest.Idaluguel, opt => opt.MapFrom(src => src.AluguelId ?? 0))
                .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.DataHoraPagamento ?? DateTime.Now))
                
                .ForMember(dest => dest.IdaluguelNavigation, opt => opt.Ignore());

            CreateMap<PagamentoDto, PagamentoViewModel>()
                .ForMember(dest => dest.ValorStr, opt => opt.MapFrom(src => src.Valor.ToString("F2")))
                .ForMember(dest => dest.AluguelId, opt => opt.MapFrom(src => src.Idaluguel))
                .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.DataPagamento != default(DateTime) ? DateOnly.FromDateTime(src.DataPagamento) : (DateOnly?)null))
                .ForMember(dest => dest.HoraPagamento, opt => opt.MapFrom(src => src.DataPagamento != default(DateTime) ? TimeOnly.FromDateTime(src.DataPagamento) : (TimeOnly?)null))
                .ForMember(dest => dest.DataHoraPagamento, opt => opt.MapFrom(src => src.DataPagamento))
                .ReverseMap()
                .ForMember(dest => dest.Idaluguel, opt => opt.MapFrom(src => src.AluguelId))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.DataHoraPagamento))
                .ForMember(dest => dest.TipoPagamento, opt => opt.MapFrom(src => src.TipoPagamento));
        }
    }
}