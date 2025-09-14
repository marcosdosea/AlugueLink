using AutoMapper;
using Core;
using Models;

namespace Mappers
{
    public class PagamentoProfile : Profile
    {
        public PagamentoProfile()
        {
            CreateMap<PagamentoViewModel, Pagamento>().ReverseMap();
        }
    }
}