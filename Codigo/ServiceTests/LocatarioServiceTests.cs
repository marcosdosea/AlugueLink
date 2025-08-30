using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ServiceTests
{
    public class LocatarioServiceTests : IDisposable
    {
        private readonly AluguelinkContext _context;
        private readonly LocatarioService _locatarioService;

        public LocatarioServiceTests()
        {
            // Configurar contexto in-memory para testes
            var options = new DbContextOptionsBuilder<AluguelinkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AluguelinkContext(options);
            _locatarioService = new LocatarioService(_context);
        }

        #region Create Tests - Criar Locatário

        [Fact]
        public async Task CreateAsync_DeveInserirLocatarioComSucesso()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
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

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal(locatarioDto.Nome, resultado.Nome);
            Assert.Equal(locatarioDto.Email, resultado.Email);
            Assert.Equal(locatarioDto.Telefone1, resultado.Telefone1);
            Assert.Equal(locatarioDto.Telefone2, resultado.Telefone2);
            Assert.Equal(locatarioDto.Cpf, resultado.Cpf);
            Assert.Equal(locatarioDto.Cep, resultado.Cep);
            Assert.Equal(locatarioDto.Logradouro, resultado.Logradouro);
            Assert.Equal(locatarioDto.Numero, resultado.Numero);
            Assert.Equal(locatarioDto.Complemento, resultado.Complemento);
            Assert.Equal(locatarioDto.Bairro, resultado.Bairro);
            Assert.Equal(locatarioDto.Cidade, resultado.Cidade);
            Assert.Equal(locatarioDto.Estado, resultado.Estado);
            Assert.Equal(locatarioDto.Profissao, resultado.Profissao);
            Assert.Equal(locatarioDto.Renda, resultado.Renda);

            // Verificar se foi salvo no banco
            var locatarioNoBanco = await _context.Locatarios
                .Where(l => l.Id == resultado.Id)
                .FirstOrDefaultAsync();
            Assert.NotNull(locatarioNoBanco);
            Assert.Equal(resultado.Nome, locatarioNoBanco.Nome);
        }

        [Fact]
        public async Task CreateAsync_ComCamposObrigatorios_DeveInserirComSucesso()
        {
            // Arrange - Apenas campos obrigatórios
            var locatarioDto = new LocatarioDTO
            {
                Nome = "Maria Santos",
                Email = "maria.santos@email.com",
                Telefone1 = "21987654321",
                Telefone2 = "", // Campo obrigatório mas pode ser string vazia
                Cpf = "98765432100",
                Cep = "20000000",
                Logradouro = "Av. Copacabana",
                Numero = "1000",
                Bairro = "Copacabana",
                Cidade = "Rio de Janeiro",
                Estado = "RJ"
                // Profissão e Renda são opcionais, Complemento é opcional
            };

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal("Maria Santos", resultado.Nome);
            Assert.Equal("maria.santos@email.com", resultado.Email);
            Assert.Equal("98765432100", resultado.Cpf);
            Assert.Equal("Rio de Janeiro", resultado.Cidade);
            Assert.Equal("RJ", resultado.Estado);
            Assert.Null(resultado.Profissao);
            Assert.Null(resultado.Renda);
        }

        [Fact]
        public async Task CreateAsync_ComCamposOpcionaisNulos_DeveFuncionar()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Nome = "Carlos Pereira",
                Email = "carlos@email.com",
                Telefone1 = "31987654321",
                Telefone2 = "3133445566",
                Cpf = "11122233344",
                Cep = "30000000",
                Logradouro = "Rua da Bahia",
                Numero = "500",
                Complemento = null, // Campo opcional
                Bairro = "Centro",
                Cidade = "Belo Horizonte",
                Estado = "MG",
                Profissao = null, // Campo opcional
                Renda = null // Campo opcional
            };

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal("Carlos Pereira", resultado.Nome);
            Assert.Null(resultado.Complemento);
            Assert.Null(resultado.Profissao);
            Assert.Null(resultado.Renda);
        }

        [Theory]
        [InlineData("SP")]
        [InlineData("RJ")]
        [InlineData("MG")]
        [InlineData("RS")]
        public async Task CreateAsync_ComDiferentesEstados_DeveInserirCorretamente(string estado)
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Nome = "Teste Estado",
                Email = "teste@email.com",
                Telefone1 = "11999999999",
                Telefone2 = "1133333333",
                Cpf = "12312312312",
                Cep = "12345678",
                Logradouro = "Rua Teste",
                Numero = "100",
                Bairro = "Centro",
                Cidade = "Cidade Teste",
                Estado = estado
            };

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(estado, resultado.Estado);
            
            var locatarioNoBanco = await _context.Locatarios
                .Where(l => l.Id == resultado.Id)
                .FirstOrDefaultAsync();
            Assert.Equal(estado, locatarioNoBanco?.Estado);
        }

        [Fact]
        public async Task CreateAsync_ComRendaDecimal_DevePreservarCasasDecimais()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Nome = "Teste Renda",
                Email = "renda@email.com",
                Telefone1 = "11888888888",
                Telefone2 = "1122222222",
                Cpf = "99988877766",
                Cep = "87654321",
                Logradouro = "Av. Renda",
                Numero = "999",
                Bairro = "Vila Rica",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Contador",
                Renda = 7845.67m // Precisão decimal
            };

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.Equal(7845.67m, resultado.Renda);
            
            var locatarioNoBanco = await _context.Locatarios
                .Where(l => l.Id == resultado.Id)
                .FirstOrDefaultAsync();
            Assert.Equal(7845.67m, locatarioNoBanco?.Renda);
        }

        [Fact]
        public async Task CreateAsync_DeveAtribuirIdAutomaticamente()
        {
            // Arrange
            var locatario1 = new LocatarioDTO 
            { 
                Nome = "Locatário 1", Email = "loc1@email.com", Telefone1 = "11111111111",
                Telefone2 = "1111111111", Cpf = "11111111111", Cep = "11111111",
                Logradouro = "Rua 1", Numero = "1", Bairro = "B1", Cidade = "C1", Estado = "SP"
            };
            var locatario2 = new LocatarioDTO 
            { 
                Nome = "Locatário 2", Email = "loc2@email.com", Telefone1 = "22222222222",
                Telefone2 = "2222222222", Cpf = "22222222222", Cep = "22222222",
                Logradouro = "Rua 2", Numero = "2", Bairro = "B2", Cidade = "C2", Estado = "RJ"
            };

            // Act
            var resultado1 = await _locatarioService.CreateAsync(locatario1);
            var resultado2 = await _locatarioService.CreateAsync(locatario2);

            // Assert
            Assert.True(resultado1.Id > 0);
            Assert.True(resultado2.Id > 0);
            Assert.NotEqual(resultado1.Id, resultado2.Id);
        }

        #endregion

        #region Index Tests - Meus Locatários

        [Fact]
        public async Task GetAllAsync_DeveBuscarTodosLocatarios()
        {
            // Arrange
            var locatario1 = new LocatarioDTO
            {
                Nome = "Ana Costa", Email = "ana@email.com", Telefone1 = "11111111111",
                Telefone2 = "1111111111", Cpf = "11111111111", Cep = "11111111",
                Logradouro = "Rua Ana", Numero = "100", Bairro = "Vila Ana",
                Cidade = "São Paulo", Estado = "SP", Profissao = "Médica", Renda = 8000m
            };
            var locatario2 = new LocatarioDTO
            {
                Nome = "Bruno Lima", Email = "bruno@email.com", Telefone1 = "22222222222",
                Telefone2 = "2222222222", Cpf = "22222222222", Cep = "22222222",
                Logradouro = "Av. Bruno", Numero = "200", Bairro = "Centro",
                Cidade = "Rio de Janeiro", Estado = "RJ", Profissao = "Advogado", Renda = 6500m
            };

            await _locatarioService.CreateAsync(locatario1);
            await _locatarioService.CreateAsync(locatario2);

            // Act
            var resultado = await _locatarioService.GetAllAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Count() >= 2);
            
            var lista = resultado.ToList();
            Assert.Contains(lista, l => l.Nome == "Ana Costa" && l.Profissao == "Médica");
            Assert.Contains(lista, l => l.Nome == "Bruno Lima" && l.Profissao == "Advogado");
        }

        [Fact]
        public async Task GetAllAsync_SemLocatarios_DeveRetornarListaVazia()
        {
            // Act
            var resultado = await _locatarioService.GetAllAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosCamposPreenchidos()
        {
            // Arrange
            var locatarioCompleto = new LocatarioDTO
            {
                Nome = "Locatário Completo",
                Email = "completo@email.com",
                Telefone1 = "11987654321",
                Telefone2 = "1133445566",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Completa",
                Numero = "999",
                Complemento = "Casa dos fundos",
                Bairro = "Bairro Completo",
                Cidade = "Cidade Completa",
                Estado = "SP",
                Profissao = "Profissão Completa",
                Renda = 9999.99m
            };
            
            await _locatarioService.CreateAsync(locatarioCompleto);

            // Act
            var resultado = await _locatarioService.GetAllAsync();

            // Assert
            var locatario = resultado.First();
            Assert.Equal("Locatário Completo", locatario.Nome);
            Assert.Equal("completo@email.com", locatario.Email);
            Assert.Equal("11987654321", locatario.Telefone1);
            Assert.Equal("1133445566", locatario.Telefone2);
            Assert.Equal("12345678901", locatario.Cpf);
            Assert.Equal("01234567", locatario.Cep);
            Assert.Equal("Rua Completa", locatario.Logradouro);
            Assert.Equal("999", locatario.Numero);
            Assert.Equal("Casa dos fundos", locatario.Complemento);
            Assert.Equal("Bairro Completo", locatario.Bairro);
            Assert.Equal("Cidade Completa", locatario.Cidade);
            Assert.Equal("SP", locatario.Estado);
            Assert.Equal("Profissão Completa", locatario.Profissao);
            Assert.Equal(9999.99m, locatario.Renda);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarLocatario()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Nome = "Teste GetById", Email = "getbyid@email.com", Telefone1 = "11888888888",
                Telefone2 = "1122222222", Cpf = "99988877766", Cep = "87654321",
                Logradouro = "Rua GetById", Numero = "123", Bairro = "Vila Teste",
                Cidade = "São Paulo", Estado = "SP", Profissao = "Testador", Renda = 4500m
            };
            var criado = await _locatarioService.CreateAsync(locatarioDto);

            // Act
            var resultado = await _locatarioService.GetByIdAsync(criado.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(criado.Id, resultado.Id);
            Assert.Equal("Teste GetById", resultado.Nome);
            Assert.Equal("getbyid@email.com", resultado.Email);
            Assert.Equal("Testador", resultado.Profissao);
            Assert.Equal(4500m, resultado.Renda);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Act
            var resultado = await _locatarioService.GetByIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarLocatario()
        {
            // Arrange
            var locatarioOriginal = new LocatarioDTO
            {
                Nome = "Nome Original", Email = "original@email.com", Telefone1 = "11111111111",
                Telefone2 = "1111111111", Cpf = "11111111111", Cep = "11111111",
                Logradouro = "Rua Original", Numero = "100", Bairro = "Bairro Original",
                Cidade = "Cidade Original", Estado = "SP", Profissao = "Profissão Original", Renda = 3000m
            };
            var criado = await _locatarioService.CreateAsync(locatarioOriginal);

            var locatarioAtualizado = new LocatarioDTO
            {
                Id = criado.Id,
                Nome = "Nome Atualizado", Email = "atualizado@email.com", Telefone1 = "22222222222",
                Telefone2 = "2222222222", Cpf = "22222222222", Cep = "22222222",
                Logradouro = "Rua Atualizada", Numero = "200", Bairro = "Bairro Atualizado",
                Cidade = "Cidade Atualizada", Estado = "RJ", Profissao = "Profissão Atualizada", Renda = 5000m
            };

            // Act
            var resultado = await _locatarioService.UpdateAsync(criado.Id, locatarioAtualizado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(criado.Id, resultado.Id);
            Assert.Equal("Nome Atualizado", resultado.Nome);
            Assert.Equal("atualizado@email.com", resultado.Email);
            Assert.Equal("22222222222", resultado.Telefone1);
            Assert.Equal("22222222222", resultado.Cpf);
            Assert.Equal("Cidade Atualizada", resultado.Cidade);
            Assert.Equal("RJ", resultado.Estado);
            Assert.Equal("Profissão Atualizada", resultado.Profissao);
            Assert.Equal(5000m, resultado.Renda);

            // Verificar no banco
            var locatarioNoBanco = await _context.Locatarios
                .Where(l => l.Id == criado.Id)
                .FirstOrDefaultAsync();
            Assert.NotNull(locatarioNoBanco);
            Assert.Equal("Nome Atualizado", locatarioNoBanco.Nome);
            Assert.Equal("Cidade Atualizada", locatarioNoBanco.Cidade);
        }

        [Fact]
        public async Task UpdateAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Id = 999,
                Nome = "Teste",
                Email = "teste@email.com"
            };

            // Act
            var resultado = await _locatarioService.UpdateAsync(999, locatarioDto);

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveRemoverLocatario()
        {
            // Arrange
            var locatarioDto = new LocatarioDTO
            {
                Nome = "Locatário Delete", Email = "delete@email.com", Telefone1 = "11999999999",
                Telefone2 = "1133333333", Cpf = "99999999999", Cep = "99999999",
                Logradouro = "Rua Delete", Numero = "999", Bairro = "Vila Delete",
                Cidade = "Cidade Delete", Estado = "SP"
            };
            var criado = await _locatarioService.CreateAsync(locatarioDto);

            // Act
            var resultado = await _locatarioService.DeleteAsync(criado.Id);

            // Assert
            Assert.True(resultado);

            // Verificar se foi removido do banco
            var locatarioNoBanco = await _context.Locatarios
                .Where(l => l.Id == criado.Id)
                .FirstOrDefaultAsync();
            Assert.Null(locatarioNoBanco);
        }

        [Fact]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            // Act
            var resultado = await _locatarioService.DeleteAsync(999);

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Validation Edge Cases

        [Fact]
        public async Task CreateAsync_ComValoresLimiteSuperior_DeveFuncionar()
        {
            // Arrange - Valores no limite máximo das validações
            var locatarioDto = new LocatarioDTO
            {
                Nome = new string('A', 100), // 100 caracteres (limite)
                Email = new string('b', 90) + "@teste.com", // ~100 caracteres
                Telefone1 = "11999999999", // 11 caracteres
                Telefone2 = "11888888888", // 11 caracteres
                Cpf = "12345678901", // 11 caracteres
                Cep = "12345678", // 8 caracteres
                Logradouro = new string('C', 100), // 100 caracteres
                Numero = "99999", // 5 caracteres
                Complemento = new string('D', 50), // 50 caracteres
                Bairro = new string('E', 50), // 50 caracteres
                Cidade = new string('F', 45), // 45 caracteres
                Estado = "SP", // 2 caracteres
                Profissao = new string('G', 100), // 100 caracteres
                Renda = 999999.99m // Valor máximo
            };

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal(100, resultado.Nome.Length);
            Assert.Equal(999999.99m, resultado.Renda);
        }

        [Fact]
        public async Task CreateAsync_ComValoresMinimos_DeveFuncionar()
        {
            // Arrange - Valores mínimos válidos
            var locatarioDto = new LocatarioDTO
            {
                Nome = "A", // 1 caractere (mínimo)
                Email = "a@b.co", // Email mínimo válido
                Telefone1 = "1199999999", // 10 caracteres (mínimo)
                Telefone2 = "1188888888", // 10 caracteres (mínimo)
                Cpf = "12345678901", // 11 caracteres (obrigatório)
                Cep = "12345678", // 8 caracteres (obrigatório)
                Logradouro = "R", // 1 caractere (mínimo)
                Numero = "1", // 1 caractere (mínimo)
                Bairro = "B", // 1 caractere (mínimo)
                Cidade = "C", // 1 caractere (mínimo)
                Estado = "SP", // 2 caracteres (obrigatório)
                Renda = 0.01m // Valor mínimo válido
            };

            // Act
            var resultado = await _locatarioService.CreateAsync(locatarioDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal("A", resultado.Nome);
            Assert.Equal("a@b.co", resultado.Email);
            Assert.Equal(0.01m, resultado.Renda);
        }

        #endregion

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}