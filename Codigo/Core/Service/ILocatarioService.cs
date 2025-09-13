using Core.DTO;

namespace Core.Service
{
    public interface ILocatarioService
    {
        int Create(Locatario locatario);
        
        void Edit(Locatario locatario);
        
        void Delete(int id);
        
        Locatario? Get(int id);
        
        IEnumerable<Locatario> GetAll(int page, int pageSize);
        
        IEnumerable<LocatarioDto> GetByCpf(string cpf);
        
        IEnumerable<LocatarioDto> GetByNome(string nome);
        
        int GetCount();
    }
}