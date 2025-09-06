using Core.DTO;

namespace Core.Service
{
    public interface IImovelService
    {
        int Create(Imovel imovel);
        void Edit(Imovel imovel);
        void Delete(int id);
        Imovel? Get(int id);
        IEnumerable<Imovel> GetAll(int page, int pageSize);
        IEnumerable<ImovelDTO> GetByLocador(int idLocador);
        IEnumerable<ImovelDTO> GetByTipo(string tipo);
        IEnumerable<ImovelDTO> GetByValorRange(decimal valorMin, decimal valorMax);
        int GetCount();
    }
}