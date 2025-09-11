using AutoMapper;
using Core.DTO;
using Core;

namespace AlugueLinkWEB.Mappers
{
    /// <summary>
    /// Perfil AutoMapper para Pagamento
    /// </summary>
    public class PagamentoProfile : Profile
    {
        public PagamentoProfile()
        {
            CreateMap<PagamentoDto, Pagamento>()
                .ReverseMap();
        }
    }
}