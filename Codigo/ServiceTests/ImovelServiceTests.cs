using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ServiceTests
{
    public class ImovelServiceTests : IDisposable
    {
        private readonly AluguelinkContext _context;
        private readonly ImovelService _imovelService;

        public ImovelServiceTests()
        {
            // Configurar contexto in-memory para testes
            var options = new DbContextOptionsBuilder<AluguelinkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AluguelinkContext(options);
            _imovelService = new ImovelService(_context);

            // Seed data - criar locador para testes
            SeedTestData();
        }

        private void SeedTestData()
        {
            var locador = new Locador
            {
                Id = 1,
                Nome = "Locador Teste",
                Email = "locador@teste.com",
                Cpf = "12345678901",
                Telefone = "11999999999"
            };

            _context.Locadors.Add(locador);
            _context.SaveChanges();
        }

        #region Create Tests

        [Fact]
        public async Task CreateAsync_DeveInserirImovelComSucesso()
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Cep = "12345678",
                Logradouro = "Rua das Flores",
                Numero = "123",
                Complemento = "Apto 1",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Tipo = "apartamento",
                Quartos = 2,
                Banheiros = 1,
                Area = 65.50m,
                VagasGaragem = 1,
                Valor = 1500.00m,
                Descricao = "Apartamento bem localizado",
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal(imovelDto.Cep, resultado.Cep);
            Assert.Equal(imovelDto.Logradouro, resultado.Logradouro);
            Assert.Equal(imovelDto.Numero, resultado.Numero);
            Assert.Equal(imovelDto.Complemento, resultado.Complemento);
            Assert.Equal(imovelDto.Bairro, resultado.Bairro);
            Assert.Equal(imovelDto.Cidade, resultado.Cidade);
            Assert.Equal(imovelDto.Estado, resultado.Estado);
            Assert.Equal(imovelDto.Tipo, resultado.Tipo);
            Assert.Equal(imovelDto.Quartos, resultado.Quartos);
            Assert.Equal(imovelDto.Banheiros, resultado.Banheiros);
            Assert.Equal(imovelDto.Area, resultado.Area);
            Assert.Equal(imovelDto.VagasGaragem, resultado.VagasGaragem);
            Assert.Equal(imovelDto.Valor, resultado.Valor);
            Assert.Equal(imovelDto.Descricao, resultado.Descricao);
            Assert.Equal(imovelDto.IdLocador, resultado.IdLocador);

            // Verificar se foi salvo no banco (chave composta)
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == resultado.Id && i.IdLocador == resultado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.NotNull(imovelNoBanco);
            Assert.Equal(resultado.Id, imovelNoBanco.Id);
        }

        [Fact]
        public async Task CreateAsync_ComCamposObrigatorios_DeveInserirComSucesso()
        {
            // Arrange - Apenas campos obrigatórios segundo as validações do ViewModel
            var imovelDto = new ImovelDTO
            {
                Cep = "01234567",
                Logradouro = "Av. Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                Tipo = "casa",
                Quartos = 3,
                Banheiros = 2,
                Area = 150.75m,
                Valor = 3500.00m,
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal(imovelDto.IdLocador, resultado.IdLocador);
            Assert.Equal(imovelDto.Tipo, resultado.Tipo);
            Assert.Equal(imovelDto.Cidade, resultado.Cidade);
            Assert.Equal(imovelDto.Valor, resultado.Valor);
            Assert.Equal(imovelDto.Cep, resultado.Cep);
            Assert.Equal(imovelDto.Logradouro, resultado.Logradouro);
            Assert.Equal(imovelDto.Numero, resultado.Numero);
            Assert.Equal(imovelDto.Bairro, resultado.Bairro);
            Assert.Equal(imovelDto.Estado, resultado.Estado);
            Assert.Equal(imovelDto.Quartos, resultado.Quartos);
            Assert.Equal(imovelDto.Banheiros, resultado.Banheiros);
            Assert.Equal(imovelDto.Area, resultado.Area);

            // Verificar se foi salvo no banco (usar Where em vez de Find para chave composta)
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == resultado.Id && i.IdLocador == resultado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.NotNull(imovelNoBanco);
        }

        [Theory]
        [InlineData("casa")]
        [InlineData("apartamento")]
        [InlineData("comercial")]
        public async Task CreateAsync_ComDiferentesTiposDeImovel_DeveInserirCorretamente(string tipoImovel)
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Cep = "12345678",
                Logradouro = "Rua Teste",
                Numero = "100",
                Bairro = "Centro",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                Tipo = tipoImovel,
                Quartos = 2,
                Banheiros = 1,
                Area = 80.00m,
                Valor = 2000.00m,
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tipoImovel, resultado.Tipo);
            
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == resultado.Id && i.IdLocador == resultado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.Equal(tipoImovel, imovelNoBanco?.Tipo);
        }

        [Fact]
        public async Task CreateAsync_ComValoresDecimaisPrecisao_DevePreservarCasasDecimais()
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Cep = "87654321",
                Logradouro = "Rua Decimal",
                Numero = "999",
                Bairro = "Teste",
                Cidade = "Belo Horizonte",
                Estado = "MG",
                Tipo = "apartamento",
                Quartos = 1,
                Banheiros = 1,
                Area = 45.75m, // Precisão decimal
                VagasGaragem = 1,
                Valor = 1234.56m, // Precisão decimal
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.Equal(45.75m, resultado.Area);
            Assert.Equal(1234.56m, resultado.Valor);
            
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == resultado.Id && i.IdLocador == resultado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.Equal(45.75m, imovelNoBanco?.Area);
            Assert.Equal(1234.56m, imovelNoBanco?.Valor);
        }

        [Fact]
        public async Task CreateAsync_ComCamposOpcionaisNulos_DeveUtilizarValoresPadrao()
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Cep = "11111111",
                Logradouro = "Rua Obrigatória",
                Numero = "1",
                Bairro = "Obrigatório",
                Cidade = "Cidade Obrigatória",
                Estado = "RJ",
                Tipo = "casa",
                Quartos = 1,
                Banheiros = 1,
                Area = 50.00m,
                Valor = 1000.00m,
                IdLocador = 1,
                // Campos opcionais ficam nulos
                Complemento = null,
                VagasGaragem = null,
                Descricao = null
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == resultado.Id && i.IdLocador == resultado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.NotNull(imovelNoBanco);
            Assert.Equal(0, imovelNoBanco.VagasGaragem); // Valor padrão
            Assert.Equal("", imovelNoBanco.Descricao); // Valor padrão
            // Complemento pode ser nulo, então não verificamos valor padrão
        }

        [Fact]
        public async Task CreateAsync_DeveAtribuirIdAutomaticamente()
        {
            // Arrange
            var imovel1 = new ImovelDTO 
            { 
                Cep = "11111111", Logradouro = "Rua 1", Numero = "1", Bairro = "B1",
                Cidade = "Cidade1", Estado = "SP", Tipo = "casa", Quartos = 1, 
                Banheiros = 1, Area = 50m, Valor = 1000m, IdLocador = 1 
            };
            var imovel2 = new ImovelDTO 
            { 
                Cep = "22222222", Logradouro = "Rua 2", Numero = "2", Bairro = "B2",
                Cidade = "Cidade2", Estado = "RJ", Tipo = "apartamento", Quartos = 2, 
                Banheiros = 1, Area = 60m, Valor = 2000m, IdLocador = 1 
            };

            // Act
            var resultado1 = await _imovelService.CreateAsync(imovel1);
            var resultado2 = await _imovelService.CreateAsync(imovel2);

            // Assert
            Assert.True(resultado1.Id > 0);
            Assert.True(resultado2.Id > 0);
            Assert.NotEqual(resultado1.Id, resultado2.Id);
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_DeveBuscarTodosImoveisComLocadores()
        {
            // Arrange
            var imovel1 = new ImovelDTO
            {
                Cep = "11111111", Logradouro = "Rua 1", Numero = "1", Bairro = "B1",
                Cidade = "São Paulo", Estado = "SP", Tipo = "casa", Quartos = 2,
                Banheiros = 1, Area = 80m, Valor = 1500m, IdLocador = 1
            };
            var imovel2 = new ImovelDTO
            {
                Cep = "22222222", Logradouro = "Rua 2", Numero = "2", Bairro = "B2",
                Cidade = "Rio de Janeiro", Estado = "RJ", Tipo = "apartamento", Quartos = 3,
                Banheiros = 2, Area = 90m, Valor = 2500m, IdLocador = 1
            };

            await _imovelService.CreateAsync(imovel1);
            await _imovelService.CreateAsync(imovel2);

            // Act
            var resultado = await _imovelService.GetAllAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Count() >= 2);
            
            var lista = resultado.ToList();
            Assert.Contains(lista, i => i.Cidade == "São Paulo" && i.Tipo == "casa");
            Assert.Contains(lista, i => i.Cidade == "Rio de Janeiro" && i.Tipo == "apartamento");
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarImovel()
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Cep = "12345678", Logradouro = "Rua Teste", Numero = "123", Bairro = "Centro",
                Cidade = "Brasília", Estado = "DF", Tipo = "comercial", Quartos = 0,
                Banheiros = 1, Area = 45m, Valor = 3000m, IdLocador = 1
            };
            var criado = await _imovelService.CreateAsync(imovelDto);

            // Act
            var resultado = await _imovelService.GetByIdAsync(criado.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(criado.Id, resultado.Id);
            Assert.Equal("Brasília", resultado.Cidade);
            Assert.Equal("comercial", resultado.Tipo);
            Assert.Equal(3000m, resultado.Valor);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Act
            var resultado = await _imovelService.GetByIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarImovel()
        {
            // Arrange
            var imovelOriginal = new ImovelDTO
            {
                Cep = "11111111", Logradouro = "Rua Original", Numero = "100", Bairro = "Centro",
                Cidade = "São Paulo", Estado = "SP", Tipo = "casa", Quartos = 2,
                Banheiros = 1, Area = 80m, Valor = 2000m, IdLocador = 1
            };
            var criado = await _imovelService.CreateAsync(imovelOriginal);

            var imovelAtualizado = new ImovelDTO
            {
                Id = criado.Id,
                Cep = "22222222", Logradouro = "Rua Atualizada", Numero = "200", Bairro = "Vila Nova",
                Cidade = "Rio de Janeiro", Estado = "RJ", Tipo = "apartamento", Quartos = 3,
                Banheiros = 2, Area = 100m, Valor = 3500m, IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.UpdateAsync(criado.Id, imovelAtualizado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(criado.Id, resultado.Id);
            Assert.Equal("22222222", resultado.Cep);
            Assert.Equal("Rua Atualizada", resultado.Logradouro);
            Assert.Equal("200", resultado.Numero);
            Assert.Equal("Vila Nova", resultado.Bairro);
            Assert.Equal("Rio de Janeiro", resultado.Cidade);
            Assert.Equal("RJ", resultado.Estado);
            Assert.Equal("apartamento", resultado.Tipo);
            Assert.Equal(3, resultado.Quartos);
            Assert.Equal(2, resultado.Banheiros);
            Assert.Equal(100m, resultado.Area);
            Assert.Equal(3500m, resultado.Valor);

            // Verificar no banco
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == criado.Id && i.IdLocador == criado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.NotNull(imovelNoBanco);
            Assert.Equal("Rio de Janeiro", imovelNoBanco.Cidade);
            Assert.Equal("apartamento", imovelNoBanco.Tipo);
        }

        [Fact]
        public async Task UpdateAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Id = 999,
                Cidade = "Teste",
                Valor = 1000m,
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.UpdateAsync(999, imovelDto);

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveRemoverImovel()
        {
            // Arrange
            var imovelDto = new ImovelDTO
            {
                Cep = "99999999", Logradouro = "Rua Delete", Numero = "999", Bairro = "Test",
                Cidade = "Cidade Delete", Estado = "SP", Tipo = "casa", Quartos = 1,
                Banheiros = 1, Area = 50m, Valor = 1200m, IdLocador = 1
            };
            var criado = await _imovelService.CreateAsync(imovelDto);

            // Act
            var resultado = await _imovelService.DeleteAsync(criado.Id);

            // Assert
            Assert.True(resultado);

            // Verificar se foi removido do banco
            var imovelNoBanco = await _context.Imovels
                .Where(i => i.Id == criado.Id && i.IdLocador == criado.IdLocador)
                .FirstOrDefaultAsync();
            Assert.Null(imovelNoBanco);
        }

        [Fact]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            // Act
            var resultado = await _imovelService.DeleteAsync(999);

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Validation Edge Cases

        [Fact]
        public async Task CreateAsync_ComValoresLimiteSuperior_DeveFuncionar()
        {
            // Arrange - Valores no limite máximo das validações
            var imovelDto = new ImovelDTO
            {
                Cep = "87654321", // 8 caracteres
                Logradouro = new string('A', 100), // 100 caracteres (limite)
                Numero = "99999", // 5 caracteres (limite)
                Complemento = new string('B', 50), // 50 caracteres (limite)
                Bairro = new string('C', 50), // 50 caracteres (limite)
                Cidade = new string('D', 45), // 45 caracteres (limite)
                Estado = "SP", // 2 caracteres
                Tipo = "apartamento",
                Quartos = 50, // Limite máximo
                Banheiros = 50, // Limite máximo
                Area = 99999.99m, // Limite máximo
                VagasGaragem = 50, // Limite máximo
                Valor = 999999.99m, // Limite máximo
                Descricao = "Descrição teste",
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal(50, resultado.Quartos);
            Assert.Equal(50, resultado.Banheiros);
            Assert.Equal(99999.99m, resultado.Area);
            Assert.Equal(50, resultado.VagasGaragem);
            Assert.Equal(999999.99m, resultado.Valor);
        }

        [Fact]
        public async Task CreateAsync_ComValoresMinimos_DeveFuncionar()
        {
            // Arrange - Valores mínimos válidos
            var imovelDto = new ImovelDTO
            {
                Cep = "1", // 1 caractere (mínimo)
                Logradouro = "R", // 1 caractere (mínimo)
                Numero = "1", // 1 caractere (mínimo)
                Bairro = "B", // 1 caractere (mínimo)
                Cidade = "C", // 1 caractere (mínimo)
                Estado = "SP", // 2 caracteres (obrigatório)
                Tipo = "casa",
                Quartos = 0, // Valor mínimo
                Banheiros = 0, // Valor mínimo
                Area = 0.01m, // Valor mínimo válido
                VagasGaragem = 0, // Valor mínimo
                Valor = 0.01m, // Valor mínimo válido
                IdLocador = 1
            };

            // Act
            var resultado = await _imovelService.CreateAsync(imovelDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Id > 0);
            Assert.Equal(0, resultado.Quartos);
            Assert.Equal(0, resultado.Banheiros);
            Assert.Equal(0.01m, resultado.Area);
            Assert.Equal(0, resultado.VagasGaragem);
            Assert.Equal(0.01m, resultado.Valor);
        }

        #endregion

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}