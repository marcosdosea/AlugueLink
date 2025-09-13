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
        
        IEnumerable<ImovelDto> GetByLocador(int idLocador);
        
        IEnumerable<ImovelDto> GetByTipo(string tipo);
        
        IEnumerable<ImovelDto> GetByValorRange(decimal valorMin, decimal valorMax);
        
        int GetCount();
    }
}