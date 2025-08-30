using Microsoft.AspNetCore.Mvc;
using Core.Service;
using Core.DTO;
using AlugueLinkWEB.Controllers;
using AlugueLinkWEB.Models;
using AlugueLinkWEB.Mappers;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AlugueLinkWebTests
{
    public class LocatarioControllerTests
    {
        private readonly Mock<ILocatarioService> _mockLocatarioService;
        private readonly LocatarioController _controller;

        public LocatarioControllerTests()
        {
            _mockLocatarioService = new Mock<ILocatarioService>();
            _controller = new LocatarioController(_mockLocatarioService.Object);
            
            // Setup TempData para testes que precisam
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        #region Index Tests - Meus Locatários

        [Fact]
        public async Task Index_DeveRetornarViewComListaDeLocatarios()
        {
            // Arrange
            var locatarios = new List<LocatarioDTO>
            {
                new LocatarioDTO 
                { 
                    Id = 1, Nome = "João Silva", Email = "joao@email.com", 
                    Telefone1 = "11999999999", Telefone2 = "1133333333", Cpf = "12345678901",
                    Cep = "01234567", Logradouro = "Rua A", Numero = "123", Bairro = "Centro",
                    Cidade = "São Paulo", Estado = "SP", Profissao = "Engenheiro", Renda = 5000m
                },
                new LocatarioDTO 
                { 
                    Id = 2, Nome = "Maria Santos", Email = "maria@email.com", 
                    Telefone1 = "21888888888", Telefone2 = "2122222222", Cpf = "98765432100",
                    Cep = "20000000", Logradouro = "Av. B", Numero = "456", Bairro = "Copacabana",
                    Cidade = "Rio de Janeiro", Estado = "RJ", Profissao = "Médica", Renda = 8000m
                }
            };

            _mockLocatarioService.Setup(s => s.GetAllAsync())
                               .ReturnsAsync(locatarios);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<LocatarioViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            Assert.Contains(model, l => l.Nome == "João Silva");
            Assert.Contains(model, l => l.Nome == "Maria Santos");

            _mockLocatarioService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Index_ComListaVazia_DeveRetornarViewComListaVazia()
        {
            // Arrange
            var locatarios = new List<LocatarioDTO>();

            _mockLocatarioService.Setup(s => s.GetAllAsync())
                               .ReturnsAsync(locatarios);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<LocatarioViewModel>>(viewResult.Model);
            Assert.Empty(model);

            _mockLocatarioService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Index_DeveConverterDTOsParaViewModelsCorretamente()
        {
            // Arrange
            var locatarios = new List<LocatarioDTO>
            {
                new LocatarioDTO 
                { 
                    Id = 1, Nome = "Teste Conversão", Email = "conversao@email.com", 
                    Telefone1 = "11987654321", Telefone2 = "1133445566", Cpf = "12345678901",
                    Cep = "01234567", Logradouro = "Rua Conversão", Numero = "999", 
                    Complemento = "Apto 10", Bairro = "Vila Teste", Cidade = "São Paulo", 
                    Estado = "SP", Profissao = "Testador", Renda = 4500.75m
                }
            };

            _mockLocatarioService.Setup(s => s.GetAllAsync())
                               .ReturnsAsync(locatarios);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<LocatarioViewModel>>(viewResult.Model);
            var locatario = model.First();

            Assert.Equal(1, locatario.Id);
            Assert.Equal("Teste Conversão", locatario.Nome);
            Assert.Equal("conversao@email.com", locatario.Email);
            Assert.Equal("11987654321", locatario.Telefone1);
            Assert.Equal("1133445566", locatario.Telefone2);
            Assert.Equal("12345678901", locatario.Cpf);
            Assert.Equal("01234567", locatario.Cep);
            Assert.Equal("Rua Conversão", locatario.Logradouro);
            Assert.Equal("999", locatario.Numero);
            Assert.Equal("Apto 10", locatario.Complemento);
            Assert.Equal("Vila Teste", locatario.Bairro);
            Assert.Equal("São Paulo", locatario.Cidade);
            Assert.Equal("SP", locatario.Estado);
            Assert.Equal("Testador", locatario.Profissao);
            Assert.Equal(4500.75m, locatario.Renda);
        }

        #endregion

        #region Create GET Tests

        [Fact]
        public void Create_GET_DeveRetornarViewVazia()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        #endregion

        #region Create POST Tests - Criar Locatário

        [Fact]
        public async Task Create_POST_ComModeloValido_DeveCriarLocatarioERedirecionarParaIndex()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
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

            var locatarioDto = new LocatarioDTO
            {
                Id = 1,
                Nome = viewModel.Nome,
                Email = viewModel.Email,
                Telefone1 = viewModel.Telefone1,
                Telefone2 = viewModel.Telefone2!,
                Cpf = viewModel.Cpf,
                Cep = viewModel.Cep,
                Logradouro = viewModel.Logradouro,
                Numero = viewModel.Numero,
                Complemento = viewModel.Complemento,
                Bairro = viewModel.Bairro,
                Cidade = viewModel.Cidade,
                Estado = viewModel.Estado,
                Profissao = viewModel.Profissao,
                Renda = viewModel.Renda
            };

            _mockLocatarioService.Setup(s => s.CreateAsync(It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.CreateAsync(It.Is<LocatarioDTO>(dto => 
                dto.Nome == "João Silva" && 
                dto.Email == "joao.silva@email.com" &&
                dto.Cpf == "12345678901")), Times.Once);
        }

        [Fact]
        public async Task Create_POST_ComModeloInvalido_DeveRetornarViewComErros()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                // Nome não preenchido (obrigatório)
                Email = "email_invalido", // Email inválido
                Telefone1 = "123", // Telefone muito curto
                Cpf = "123" // CPF inválido
                // Outros campos obrigatórios não preenchidos
            };

            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            _controller.ModelState.AddModelError("Email", "Email inválido");

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LocatarioViewModel>(viewResult.Model);
            Assert.Equal(viewModel, model);
            Assert.False(_controller.ModelState.IsValid);

            _mockLocatarioService.Verify(s => s.CreateAsync(It.IsAny<LocatarioDTO>()), Times.Never);
        }

        [Fact]
        public async Task Create_POST_ComCamposObrigatorios_DeveFuncionar()
        {
            // Arrange - Apenas campos obrigatórios preenchidos
            var viewModel = new LocatarioViewModel
            {
                Nome = "Maria Santos",
                Email = "maria@email.com",
                Telefone1 = "21987654321",
                Cpf = "98765432100",
                Cep = "20000000",
                Logradouro = "Av. Copacabana",
                Numero = "1000",
                Bairro = "Copacabana",
                Cidade = "Rio de Janeiro",
                Estado = "RJ"
                // Telefone2, Complemento, Profissao e Renda são opcionais
            };

            var locatarioDto = new LocatarioDTO { Id = 1 };
            _mockLocatarioService.Setup(s => s.CreateAsync(It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.CreateAsync(It.Is<LocatarioDTO>(dto => 
                dto.Nome == "Maria Santos" && 
                dto.Cidade == "Rio de Janeiro" &&
                dto.Estado == "RJ")), Times.Once);
        }

        [Theory]
        [InlineData("SP")]
        [InlineData("RJ")]
        [InlineData("MG")]
        [InlineData("RS")]
        public async Task Create_POST_ComDiferentesEstados_DeveFuncionar(string estado)
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Nome = "Teste Estado",
                Email = "teste@email.com",
                Telefone1 = "11999999999",
                Cpf = "12312312312",
                Cep = "12345678",
                Logradouro = "Rua Teste",
                Numero = "100",
                Bairro = "Centro",
                Cidade = "Cidade Teste",
                Estado = estado
            };

            var locatarioDto = new LocatarioDTO { Id = 1, Estado = estado };
            _mockLocatarioService.Setup(s => s.CreateAsync(It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.CreateAsync(It.Is<LocatarioDTO>(dto => 
                dto.Estado == estado)), Times.Once);
        }

        [Fact]
        public async Task Create_POST_ComRendaDecimal_DevePreservarValor()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Nome = "Teste Renda",
                Email = "renda@email.com",
                Telefone1 = "11888888888",
                Cpf = "99988877766",
                Cep = "87654321",
                Logradouro = "Av. Renda",
                Numero = "999",
                Bairro = "Vila Rica",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Contador",
                Renda = 7845.67m
            };

            var locatarioDto = new LocatarioDTO { Id = 1, Renda = 7845.67m };
            _mockLocatarioService.Setup(s => s.CreateAsync(It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.CreateAsync(It.Is<LocatarioDTO>(dto => 
                dto.Renda == 7845.67m)), Times.Once);
        }

        #endregion

        #region Details Tests

        [Fact]
        public async Task Details_ComIdValido_DeveRetornarViewComLocatario()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone1 = "11999999999",
                Telefone2 = "1133333333",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua A",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Engenheiro",
                Renda = 5000m
            };

            _mockLocatarioService.Setup(s => s.GetByIdAsync(1))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LocatarioViewModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("João Silva", model.Nome);
            Assert.Equal("joao@email.com", model.Email);

            _mockLocatarioService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task Details_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _mockLocatarioService.Setup(s => s.GetByIdAsync(999))
                               .ReturnsAsync((LocatarioDTO?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockLocatarioService.Verify(s => s.GetByIdAsync(999), Times.Once);
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task Edit_GET_ComIdValido_DeveRetornarViewComLocatario()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@email.com",
                Cpf = "12345678901"
            };

            _mockLocatarioService.Setup(s => s.GetByIdAsync(1))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LocatarioViewModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("João Silva", model.Nome);
        }

        [Fact]
        public async Task Edit_GET_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _mockLocatarioService.Setup(s => s.GetByIdAsync(999))
                               .ReturnsAsync((LocatarioDTO?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_POST_ComModeloValido_DeveAtualizarERedirecionarParaIndex()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "João Silva Atualizado",
                Email = "joao.atualizado@email.com",
                Telefone1 = "11999999999",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Atualizada",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            var locatarioDto = new LocatarioDTO { Id = 1, Nome = "João Silva Atualizado" };
            _mockLocatarioService.Setup(s => s.UpdateAsync(1, It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Edit(1, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.UpdateAsync(1, It.Is<LocatarioDTO>(dto => 
                dto.Nome == "João Silva Atualizado")), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ComIdsDiferentes_DeveRetornarNotFound()
        {
            // Arrange
            var viewModel = new LocatarioViewModel { Id = 2 };

            // Act
            var result = await _controller.Edit(1, viewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockLocatarioService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<LocatarioDTO>()), Times.Never);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_GET_ComIdValido_DeveRetornarViewComLocatario()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@email.com"
            };

            _mockLocatarioService.Setup(s => s.GetByIdAsync(1))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LocatarioViewModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("João Silva", model.Nome);
        }

        [Fact]
        public async Task Delete_GET_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _mockLocatarioService.Setup(s => s.GetByIdAsync(999))
                               .ReturnsAsync((LocatarioDTO?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ComIdValido_DeveExcluirERedirecionarParaIndex()
        {
            // Arrange
            _mockLocatarioService.Setup(s => s.DeleteAsync(1))
                               .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_ComIdInexistente_DeveRedirecionarParaIndexComErro()
        {
            // Arrange
            _mockLocatarioService.Setup(s => s.DeleteAsync(999))
                               .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteConfirmed(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.DeleteAsync(999), Times.Once);
        }

        #endregion

        #region Edge Cases e Cenários de Erro

        [Fact]
        public async Task Create_POST_ComCamposNulos_DeveConverterParaStringVazia()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Nome = "Teste Nulos",
                Email = "teste@email.com",
                Telefone1 = "11999999999",
                Telefone2 = null, // Campo opcional nulo
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Teste",
                Numero = "123",
                Complemento = null, // Campo opcional nulo
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = null, // Campo opcional nulo
                Renda = null // Campo opcional nulo
            };

            var locatarioDto = new LocatarioDTO { Id = 1 };
            _mockLocatarioService.Setup(s => s.CreateAsync(It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync(locatarioDto);

            // Act
            var result = await _controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockLocatarioService.Verify(s => s.CreateAsync(It.Is<LocatarioDTO>(dto => 
                dto.Telefone2 == string.Empty && // Deve converter null para string vazia
                dto.Complemento == null &&
                dto.Profissao == null &&
                dto.Renda == null)), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ComModeloInvalido_DeveRetornarViewComErros()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "", // Campo obrigatório vazio
                Email = "email_invalido" // Email inválido
            };

            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            _controller.ModelState.AddModelError("Email", "Email inválido");

            // Act
            var result = await _controller.Edit(1, viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LocatarioViewModel>(viewResult.Model);
            Assert.Equal(viewModel, model);
            Assert.False(_controller.ModelState.IsValid);

            _mockLocatarioService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<LocatarioDTO>()), Times.Never);
        }

        [Fact]
        public async Task Edit_POST_ComServicoRetornandoNull_DeveRetornarNotFound()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "Teste",
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

            _mockLocatarioService.Setup(s => s.UpdateAsync(1, It.IsAny<LocatarioDTO>()))
                               .ReturnsAsync((LocatarioDTO?)null);

            // Act
            var result = await _controller.Edit(1, viewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockLocatarioService.Verify(s => s.UpdateAsync(1, It.IsAny<LocatarioDTO>()), Times.Once);
        }

        #endregion
    }
}