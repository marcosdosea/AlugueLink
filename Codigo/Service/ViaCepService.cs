using Core.DTO;
using Core.Service;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Service;

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://viacep.com.br/ws/";

    public ViaCepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ViaCepDto?> BuscarEnderecoPorCepAsync(string cep)
    {
        try
        {
            var cepLimpo = LimparCep(cep);
            if (!ValidarCep(cepLimpo))
            {
                throw new ServiceException("CEP inv�lido. O CEP deve conter exatamente 8 d�gitos num�ricos.");
            }

            var url = $"{_baseUrl}{cepLimpo}/json/";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ServiceException("CEP com formato inv�lido rejeitado pela API do ViaCEP.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ServiceException($"Erro ao consultar o CEP. Status: {response.StatusCode}. Verifique sua conex�o com a internet.");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ServiceException("Resposta vazia da API do ViaCEP.");
            }

            var viaCepResponse = JsonSerializer.Deserialize<ViaCepDto>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (viaCepResponse == null)
            {
                throw new ServiceException("Erro ao processar a resposta da API do ViaCEP.");
            }

            if (viaCepResponse.Erro)
            {
                throw new ServiceException("CEP n�o encontrado na base de dados do ViaCEP.");
            }

            return viaCepResponse;
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceException("Erro de rede ao consultar o CEP. Verifique sua conex�o com a internet.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ServiceException("Timeout ao consultar o CEP. Tente novamente.", ex);
        }
        catch (JsonException ex)
        {
            throw new ServiceException("Erro ao processar a resposta da API do ViaCEP.", ex);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro inesperado ao consultar o CEP: {ex.Message}", ex);
        }
    }

    public async Task<List<ViaCepDto>> PesquisarCepPorEnderecoAsync(string uf, string cidade, string logradouro)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(uf))
            {
                throw new ServiceException("UF � obrigat�rio para pesquisa por endere�o.");
            }

            if (string.IsNullOrWhiteSpace(cidade) || cidade.Trim().Length < 3)
            {
                throw new ServiceException("Cidade � obrigat�ria e deve conter pelo menos 3 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(logradouro) || logradouro.Trim().Length < 3)
            {
                throw new ServiceException("Logradouro � obrigat�rio e deve conter pelo menos 3 caracteres.");
            }

            var ufLimpo = uf.Trim().ToUpperInvariant();
            var cidadeLimpa = Uri.EscapeDataString(cidade.Trim());
            var logradouroLimpo = Uri.EscapeDataString(logradouro.Trim());

            var url = $"{_baseUrl}{ufLimpo}/{cidadeLimpa}/{logradouroLimpo}/json/";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ServiceException("Par�metros de pesquisa inv�lidos. Verifique se cidade e logradouro possuem pelo menos 3 caracteres.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ServiceException($"Erro ao pesquisar endere�o. Status: {response.StatusCode}. Verifique sua conex�o com a internet.");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ServiceException("Resposta vazia da API do ViaCEP.");
            }

            var viaCepResponse = JsonSerializer.Deserialize<List<ViaCepDto>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (viaCepResponse == null)
            {
                throw new ServiceException("Erro ao processar a resposta da API do ViaCEP.");
            }

            var resultadosValidos = viaCepResponse.Where(x => !x.Erro).ToList();

            return resultadosValidos;
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceException("Erro de rede ao pesquisar endere�o. Verifique sua conex�o com a internet.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ServiceException("Timeout ao pesquisar endere�o. Tente novamente.", ex);
        }
        catch (JsonException ex)
        {
            throw new ServiceException("Erro ao processar a resposta da API do ViaCEP.", ex);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro inesperado ao pesquisar endere�o: {ex.Message}", ex);
        }
    }

    private static string LimparCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return string.Empty;
        }

        return Regex.Replace(cep, @"\D", "");
    }

    private static bool ValidarCep(string cep)
    {
        return !string.IsNullOrWhiteSpace(cep) && 
               cep.Length == 8 && 
               Regex.IsMatch(cep, @"^\d{8}$");
    }
}