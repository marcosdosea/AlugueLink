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

    public async Task<ViaCepDTO?> BuscarEnderecoPorCepAsync(string cep)
    {
        try
        {
            // Validar e limpar o CEP
            var cepLimpo = LimparCep(cep);
            if (!ValidarCep(cepLimpo))
            {
                throw new ServiceException("CEP inválido. O CEP deve conter 8 dígitos.");
            }

            // Fazer a requisição para a API
            var url = $"{_baseUrl}{cepLimpo}/json/";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new ServiceException("Erro ao consultar o CEP. Verifique sua conexão com a internet.");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ServiceException("Resposta vazia da API do ViaCEP.");
            }

            var viaCepResponse = JsonSerializer.Deserialize<ViaCepDTO>(jsonContent);

            if (viaCepResponse == null)
            {
                throw new ServiceException("Erro ao processar a resposta da API do ViaCEP.");
            }

            // Verificar se o CEP existe
            if (viaCepResponse.Erro)
            {
                throw new ServiceException("CEP não encontrado.");
            }

            return viaCepResponse;
        }
        catch (HttpRequestException)
        {
            throw new ServiceException("Erro de rede ao consultar o CEP. Verifique sua conexão com a internet.");
        }
        catch (TaskCanceledException)
        {
            throw new ServiceException("Timeout ao consultar o CEP. Tente novamente.");
        }
        catch (JsonException)
        {
            throw new ServiceException("Erro ao processar a resposta da API do ViaCEP.");
        }
        catch (ServiceException)
        {
            // Re-throw service exceptions
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro inesperado ao consultar o CEP: {ex.Message}");
        }
    }

    private static string LimparCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return string.Empty;
        }

        // Remove todos os caracteres que não sejam dígitos
        return Regex.Replace(cep, @"\D", "");
    }

    private static bool ValidarCep(string cep)
    {
        // Verifica se o CEP tem exatamente 8 dígitos
        return !string.IsNullOrWhiteSpace(cep) && cep.Length == 8 && Regex.IsMatch(cep, @"^\d{8}$");
    }
}