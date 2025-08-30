using AlugueLinkWEB.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace AlugueLinkWebTests
{
    public class LocatarioViewModelValidationTests
    {
        #region Nome Validation Tests

        [Fact]
        public void Nome_QuandoVazio_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = new LocatarioViewModel
            {
                Nome = "",
                Email = "teste@email.com",
                Telefone1 = "11999999999",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Teste",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Nome"));
        }

        [Fact]
        public void Nome_QuandoMuitoLongo_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = new LocatarioViewModel
            {
                Nome = new string('A', 101), // 101 caracteres (limite é 100)
                Email = "teste@email.com",
                Telefone1 = "11999999999",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Teste",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Nome"));
        }

        [Fact]
        public void Nome_ComTamanhoValido_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Nome = "João Silva"; // Nome válido

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Nome"));
        }

        #endregion

        #region Email Validation Tests

        [Fact]
        public void Email_QuandoVazio_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Email = "";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData("email_invalido")]
        [InlineData("@email.com")]
        [InlineData("email@")]
        [InlineData("email.com")]
        public void Email_ComFormatoInvalido_DeveRetornarErroDeValidacao(string emailInvalido)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Email = emailInvalido;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData("teste@email.com")]
        [InlineData("joao.silva@empresa.com.br")]
        [InlineData("usuario123@gmail.com")]
        [InlineData("contato@site.org")]
        public void Email_ComFormatoValido_DevePassarNaValidacao(string emailValido)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Email = emailValido;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Email"));
        }

        #endregion

        #region Telefone Validation Tests

        [Fact]
        public void Telefone1_QuandoVazio_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Telefone1 = "";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Telefone1"));
        }

        [Theory]
        [InlineData("123456789")] // 9 dígitos
        [InlineData("123456789012")] // 12 dígitos
        [InlineData("11abc123456")] // Contém letras
        [InlineData("11-99999-9999")] // Contém hífen
        public void Telefone1_ComFormatoInvalido_DeveRetornarErroDeValidacao(string telefoneInvalido)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Telefone1 = telefoneInvalido;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Telefone1"));
        }

        [Theory]
        [InlineData("1199999999")] // 10 dígitos
        [InlineData("11999999999")] // 11 dígitos
        public void Telefone1_ComFormatoValido_DevePassarNaValidacao(string telefoneValido)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Telefone1 = telefoneValido;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Telefone1"));
        }

        [Fact]
        public void Telefone2_QuandoVazio_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Telefone2 = "";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Telefone2"));
        }

        [Fact]
        public void Telefone2_QuandoNulo_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Telefone2 = null;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Telefone2"));
        }

        #endregion

        #region CPF Validation Tests

        [Fact]
        public void Cpf_QuandoVazio_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Cpf = "";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Cpf"));
        }

        [Theory]
        [InlineData("1234567890")] // 10 dígitos
        [InlineData("123456789012")] // 12 dígitos
        [InlineData("123.456.789-01")] // Com formatação
        [InlineData("12345678901a")] // Contém letra
        public void Cpf_ComFormatoInvalido_DeveRetornarErroDeValidacao(string cpfInvalido)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Cpf = cpfInvalido;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Cpf"));
        }

        [Fact]
        public void Cpf_ComFormatoValido_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Cpf = "12345678901"; // 11 dígitos

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Cpf"));
        }

        #endregion

        #region CEP Validation Tests

        [Fact]
        public void Cep_QuandoVazio_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Cep = "";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Cep"));
        }

        [Theory]
        [InlineData("1234567")] // 7 dígitos
        [InlineData("123456789")] // 9 dígitos
        [InlineData("12345-678")] // Com hífen
        [InlineData("1234567a")] // Contém letra
        public void Cep_ComFormatoInvalido_DeveRetornarErroDeValidacao(string cepInvalido)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Cep = cepInvalido;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Cep"));
        }

        [Fact]
        public void Cep_ComFormatoValido_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Cep = "01234567"; // 8 dígitos

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Cep"));
        }

        #endregion

        #region Endereço Validation Tests

        [Theory]
        [InlineData("Logradouro")]
        [InlineData("Numero")]
        [InlineData("Bairro")]
        [InlineData("Cidade")]
        [InlineData("Estado")]
        public void CamposEnderecoObrigatorios_QuandoVazios_DeveRetornarErroDeValidacao(string campo)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            
            switch (campo)
            {
                case "Logradouro": model.Logradouro = ""; break;
                case "Numero": model.Numero = ""; break;
                case "Bairro": model.Bairro = ""; break;
                case "Cidade": model.Cidade = ""; break;
                case "Estado": model.Estado = ""; break;
            }

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(campo));
        }

        [Fact]
        public void Estado_ComTamanhoInvalido_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Estado = "ABC"; // 3 caracteres (deve ter 2)

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Estado"));
        }

        [Fact]
        public void Complemento_QuandoVazio_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Complemento = "";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Complemento"));
        }

        [Fact]
        public void Complemento_QuandoNulo_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Complemento = null;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Complemento"));
        }

        #endregion

        #region Renda Validation Tests

        [Fact]
        public void Renda_QuandoNula_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Renda = null;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Renda"));
        }

        [Fact]
        public void Renda_ComValorNegativo_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Renda = -100;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Renda"));
        }

        [Fact]
        public void Renda_ComValorMuitoAlto_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Renda = 1000000; // Acima do limite de 999999.99

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Renda"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1500.50)]
        [InlineData(999999.99)]
        public void Renda_ComValorValido_DevePassarNaValidacao(decimal rendaValida)
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Renda = rendaValida;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Renda"));
        }

        #endregion

        #region Profissão Validation Tests

        [Fact]
        public void Profissao_QuandoNula_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Profissao = null;

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Profissao"));
        }

        [Fact]
        public void Profissao_ComTamanhoValido_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Profissao = "Engenheiro de Software";

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Profissao"));
        }

        [Fact]
        public void Profissao_MuitoLonga_DeveRetornarErroDeValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();
            model.Profissao = new string('A', 101); // 101 caracteres (limite é 100)

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Profissao"));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void LocatarioViewModel_ComTodosCamposValidos_DevePassarNaValidacao()
        {
            // Arrange
            var model = CreateValidLocatarioViewModel();

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void LocatarioViewModel_ComCamposObrigatoriosApenas_DevePassarNaValidacao()
        {
            // Arrange
            var model = new LocatarioViewModel
            {
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone1 = "11999999999",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
                // Campos opcionais não preenchidos
            };

            // Act & Assert
            var validationResults = ValidateModel(model);
            Assert.Empty(validationResults);
        }

        #endregion

        #region Helper Methods

        private static LocatarioViewModel CreateValidLocatarioViewModel()
        {
            return new LocatarioViewModel
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao.silva@email.com",
                Telefone1 = "11987654321",
                Telefone2 = "1133445566",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = "Apto 45",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Engenheiro",
                Renda = 5000.50m
            };
        }

        private static List<ValidationResult> ValidateModel(LocatarioViewModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        #endregion
    }
}