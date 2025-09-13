using Core.DTO;

namespace Core.Service;

public interface IViaCepService
{
    Task<ViaCepDto?> BuscarEnderecoPorCepAsync(string cep);
    
    Task<List<ViaCepDto>> PesquisarCepPorEnderecoAsync(string uf, string cidade, string logradouro);
}