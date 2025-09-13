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
  
        IEnumerable<ManutencaoDto> GetByImovel(int idImovel);
 
        IEnumerable<ManutencaoDto> GetByStatus(string status);
        
        IEnumerable<ManutencaoDto> GetByPeriodo(DateTime dataInicio, DateTime dataFim);

        int GetCount();
    }
}