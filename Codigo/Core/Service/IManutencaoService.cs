using Core.DTO;

namespace Core.Service
{
    public interface IManutencaoService
    {
        int Create(Manutencao manutencao);
        void Edit(Manutencao manutencao);
        void Delete(int id);
        Manutencao? Get(int id);
        IEnumerable<Manutencao> GetAll(int page, int pageSize);
        IEnumerable<ManutencaoDTO> GetByImovel(int idImovel);
        IEnumerable<ManutencaoDTO> GetByStatus(string status);
        IEnumerable<ManutencaoDTO> GetByPeriodo(DateTime dataInicio, DateTime dataFim);
        int GetCount();
    }
}