using AlugueLinkWEB.Helpers;

namespace AlugueLinkWebTests;

[TestClass]
public class ViaCepHelperTests
{
    [TestMethod]
    public void IsValidCep_CepValido_DeveRetornarTrue()
    {
        // Arrange
        var cepValido = "01310-100";

        // Act
        var resultado = ViaCepHelper.IsValidCep(cepValido);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void IsValidCep_CepSemMascara_DeveRetornarTrue()
    {
        // Arrange
        var cepSemMascara = "01310100";

        // Act
        var resultado = ViaCepHelper.IsValidCep(cepSemMascara);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void IsValidCep_CepInvalido_DeveRetornarFalse()
    {
        // Arrange
        var cepInvalido = "123";

        // Act
        var resultado = ViaCepHelper.IsValidCep(cepInvalido);

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void IsValidCep_CepNulo_DeveRetornarFalse()
    {
        // Arrange
        string? cepNulo = null;

        // Act
        var resultado = ViaCepHelper.IsValidCep(cepNulo);

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void CleanCep_CepComMascara_DeveRemoverCaracteres()
    {
        // Arrange
        var cepComMascara = "01310-100";
        var esperado = "01310100";

        // Act
        var resultado = ViaCepHelper.CleanCep(cepComMascara);

        // Assert
        Assert.AreEqual(esperado, resultado);
    }

    [TestMethod]
    public void FormatCep_CepSemMascara_DeveAdicionarMascara()
    {
        // Arrange
        var cepSemMascara = "01310100";
        var esperado = "01310-100";

        // Act
        var resultado = ViaCepHelper.FormatCep(cepSemMascara);

        // Assert
        Assert.AreEqual(esperado, resultado);
    }

    [TestMethod]
    public void FormatCep_CepInvalido_DeveRetornarOriginal()
    {
        // Arrange
        var cepInvalido = "123";

        // Act
        var resultado = ViaCepHelper.FormatCep(cepInvalido);

        // Assert
        Assert.AreEqual(cepInvalido, resultado);
    }

    [TestMethod]
    public void GetViaCepInitScript_SemParametros_DeveRetornarScript()
    {
        // Act
        var script = ViaCepHelper.GetViaCepInitScript();

        // Assert
        Assert.IsTrue(script.Contains("ViaCepUtil.init"));
        Assert.IsTrue(script.Contains("DOMContentLoaded"));
    }
}