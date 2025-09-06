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
        IEnumerable<AluguelDTO> GetByLocatario(int idLocatario);
        IEnumerable<AluguelDTO> GetByImovel(int idImovel);
        IEnumerable<AluguelDTO> GetByStatus(string status);
        int GetCount();
    }
}