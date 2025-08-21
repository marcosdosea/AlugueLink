using Core.DTO;

namespace Core.Service
{
    public interface IImovelService
    {
        Task<IEnumerable<ImovelDTO>> GetAllAsync();
        Task<ImovelDTO?> GetByIdAsync(int id);
        Task<ImovelDTO> CreateAsync(ImovelDTO imovelDto);
        Task<ImovelDTO?> UpdateAsync(int id, ImovelDTO imovelDto);
        Task<bool> DeleteAsync(int id);
    }
}