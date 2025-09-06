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
        IEnumerable<LocatarioDTO> GetByCpf(string cpf);
        IEnumerable<LocatarioDTO> GetByNome(string nome);
        int GetCount();
    }
}