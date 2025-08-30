using Core.DTO;

namespace Core.Service
{
    public interface ILocatarioService
    {
        Task<IEnumerable<LocatarioDTO>> GetAllAsync();
        Task<LocatarioDTO?> GetByIdAsync(int id);
        Task<LocatarioDTO> CreateAsync(LocatarioDTO locatarioDto);
        Task<LocatarioDTO?> UpdateAsync(int id, LocatarioDTO locatarioDto);
        Task<bool> DeleteAsync(int id);
    }
}