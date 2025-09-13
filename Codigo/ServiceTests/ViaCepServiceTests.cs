using Core.DTO;
using Core.Service;
using Service;
using System.Text.Json;

namespace ServiceTests;

[TestClass]
public class ViaCepServiceTests
{
    private IViaCepService _viaCepService = null!;
    private HttpClient _httpClient = null!;

    [TestInitialize]
    public void Setup()
    {
        _httpClient = new HttpClient();
        _viaCepService = new ViaCepService(_httpClient);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _httpClient?.Dispose();
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepValido_DeveRetornarEndereco()
    {
        var cepTeste = "01310-100";

        var resultado = await _viaCepService.BuscarEnderecoPorCepAsync(cepTeste);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("01310-100", resultado.Cep);
        Assert.IsNotNull(resultado.Logradouro);
        Assert.AreEqual("SP", resultado.Uf);
        Assert.AreEqual("São Paulo", resultado.Localidade);
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepSemMascara_DeveRetornarEndereco()
    {
        var cepTeste = "01310100";

        var resultado = await _viaCepService.BuscarEnderecoPorCepAsync(cepTeste);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("01310-100", resultado.Cep);
        Assert.IsNotNull(resultado.Logradouro);
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepInvalido_DeveLancarExcecao()
    {
        var cepInvalido = "123";

        await Assert.ThrowsExceptionAsync<ServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepInvalido)
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepInexistente_DeveLancarExcecao()
    {
        var cepInexistente = "00000000"; 

        var exception = await Assert.ThrowsExceptionAsync<ServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepInexistente)
        );

        
        Assert.IsTrue(
            exception.Message.Contains("não encontrado") ||
            exception.Message.Contains("Erro ao processar") ||
            exception.Message.Contains("CEP não encontrado"),
            $"Mensagem atual: {exception.Message}"
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepNulo_DeveLancarExcecao()
    {
        string? cepNulo = null;

        await Assert.ThrowsExceptionAsync<ServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepNulo!)
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepVazio_DeveLancarExcecao()
    {
        var cepVazio = string.Empty;

        await Assert.ThrowsExceptionAsync<ServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepVazio)
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepComLetras_DeveLancarExcecao()
    {
        var cepComLetras = "ABC12-345";

        await Assert.ThrowsExceptionAsync<ServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepComLetras)
        );
    }
}