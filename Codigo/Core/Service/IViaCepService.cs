using Core.DTO;

namespace Core.Service;

public interface IViaCepService
{
    Task<ViaCepDTO?> BuscarEnderecoPorCepAsync(string cep);
}