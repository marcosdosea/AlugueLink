using Core.DTO;

namespace Core.Service
{
    public interface IPagamentoService
    {
        int Create(Pagamento pagamento);
        void Edit(Pagamento pagamento);
        void Delete(int id);
        Pagamento? Get(int id);
        IEnumerable<Pagamento> GetAll(int page, int pageSize);
        IEnumerable<PagamentoDTO> GetByAluguel(int idAluguel);
        IEnumerable<PagamentoDTO> GetByTipoPagamento(string tipoPagamento);
        IEnumerable<PagamentoDTO> GetByPeriodo(DateTime dataInicio, DateTime dataFim);
        int GetCount();
    }
}