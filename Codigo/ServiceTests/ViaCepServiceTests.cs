using Core.DTO;
using Core.Service;
using Service;
using System.Text.Json;
using CoreServiceException = Core.Service.ServiceException;

namespace ServiceTests;

[TestClass]
public class ViaCepServiceTests
{
    private IViaCepService _viaCepService;
    private HttpClient _httpClient;

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
        // Arrange
        var cepTeste = "01310-100"; // CEP válido da Avenida Paulista, São Paulo

        // Act
        var resultado = await _viaCepService.BuscarEnderecoPorCepAsync(cepTeste);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("01310-100", resultado.Cep);
        Assert.IsNotNull(resultado.Logradouro);
        Assert.AreEqual("SP", resultado.Uf);
        Assert.AreEqual("São Paulo", resultado.Localidade);
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepSemMascara_DeveRetornarEndereco()
    {
        // Arrange
        var cepTeste = "01310100"; // CEP válido sem máscara

        // Act
        var resultado = await _viaCepService.BuscarEnderecoPorCepAsync(cepTeste);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("01310-100", resultado.Cep);
        Assert.IsNotNull(resultado.Logradouro);
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepInvalido_DeveLancarExcecao()
    {
        // Arrange
        var cepInvalido = "123";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<CoreServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepInvalido)
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepInexistente_DeveLancarExcecao()
    {
        // Arrange
        var cepInexistente = "00000-000";

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<CoreServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepInexistente)
        );

        Assert.IsTrue(exception.Message.Contains("não encontrado"));
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepNulo_DeveLancarExcecao()
    {
        // Arrange
        string? cepNulo = null;

        // Act & Assert
        await Assert.ThrowsExceptionAsync<CoreServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepNulo!)
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepVazio_DeveLancarExcecao()
    {
        // Arrange
        var cepVazio = string.Empty;

        // Act & Assert
        await Assert.ThrowsExceptionAsync<CoreServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepVazio)
        );
    }

    [TestMethod]
    public async Task BuscarEnderecoPorCepAsync_CepComLetras_DeveLancarExcecao()
    {
        // Arrange
        var cepComLetras = "ABC12-345";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<CoreServiceException>(
            () => _viaCepService.BuscarEnderecoPorCepAsync(cepComLetras)
        );
    }
}