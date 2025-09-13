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
        
        IEnumerable<PagamentoDto> GetByAluguel(int idAluguel);
        
        IEnumerable<PagamentoDto> GetByTipoPagamento(string tipoPagamento);
        
        IEnumerable<PagamentoDto> GetByPeriodo(DateTime dataInicio, DateTime dataFim);
        
        IEnumerable<Pagamento> GetByLocador(int locadorId, int page, int pageSize);
        
        int GetCountByLocador(int locadorId);
        
        int GetCount();
    }
}