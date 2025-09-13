using Core.DTO;

namespace Core.Service
{
    public interface IAluguelService
    {
        int Create(Aluguel aluguel);

        void Edit(Aluguel aluguel);

        void Delete(int id);

        Aluguel? Get(int id);

        IEnumerable<Aluguel> GetAll(int page, int pageSize);

        IEnumerable<AluguelDto> GetByLocatario(int idLocatario);

        IEnumerable<AluguelDto> GetByImovel(int idImovel);

        IEnumerable<Aluguel> GetByLocador(int idLocador);

        IEnumerable<AluguelDto> GetByStatus(string status);

        int GetCount();
        
        
        bool IsImovelAvailable(int idImovel, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null);
        
        bool IsLocatarioAvailable(int idLocatario, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null);
        
        IEnumerable<int> GetImoveisIndisponiveis();
        
        IEnumerable<int> GetLocatariosOcupados();
        
        Aluguel? GetAluguelAtivoByImovel(int idImovel);
        
        Aluguel? GetAluguelAtivoByLocatario(int idLocatario);
        
        void AtualizarStatusAlugueis();
    }
}