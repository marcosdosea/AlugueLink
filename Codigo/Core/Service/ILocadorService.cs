using Core.DTO;

namespace Core.Service
{
    public interface ILocadorService
    {   
        int Create(Locador locador);
        
        void Edit(Locador locador);
        
        void Delete(int id);
        
        Locador? Get(int id);
        
        IEnumerable<Locador> GetAll(int page, int pageSize);
        
        IEnumerable<LocadorDto> GetByCpf(string cpf);
        
        IEnumerable<LocadorDto> GetByNome(string nome);
        
        Locador? GetByEmail(string email);
        
        int GetCount();
    }
}