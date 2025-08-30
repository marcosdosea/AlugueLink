using AlugueLinkWEB.Mappers;
using AlugueLinkWEB.Models;
using Core.DTO;
using Xunit;

namespace AlugueLinkWebTests
{
    public class LocatarioMapperTests
    {
        #region ToViewModel Tests

        [Fact]
        public void ToViewModel_ComDTOCompleto_DeveConverterCorretamente()
        {
            // Arrange
            var dto = new LocatarioDTO
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

            // Act
            var viewModel = LocatarioMapper.ToViewModel(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Nome, viewModel.Nome);
            Assert.Equal(dto.Email, viewModel.Email);
            Assert.Equal(dto.Telefone1, viewModel.Telefone1);
            Assert.Equal(dto.Telefone2, viewModel.Telefone2);
            Assert.Equal(dto.Cpf, viewModel.Cpf);
            Assert.Equal(dto.Cep, viewModel.Cep);
            Assert.Equal(dto.Logradouro, viewModel.Logradouro);
            Assert.Equal(dto.Numero, viewModel.Numero);
            Assert.Equal(dto.Complemento, viewModel.Complemento);
            Assert.Equal(dto.Bairro, viewModel.Bairro);
            Assert.Equal(dto.Cidade, viewModel.Cidade);
            Assert.Equal(dto.Estado, viewModel.Estado);
            Assert.Equal(dto.Profissao, viewModel.Profissao);
            Assert.Equal(dto.Renda, viewModel.Renda);
        }

        [Fact]
        public void ToViewModel_ComCamposOpcionaisNulos_DeveConverterCorretamente()
        {
            // Arrange
            var dto = new LocatarioDTO
            {
                Id = 1,
                Nome = "Maria Santos",
                Email = "maria@email.com",
                Telefone1 = "21987654321",
                Telefone2 = "2133445566",
                Cpf = "98765432100",
                Cep = "20000000",
                Logradouro = "Av. Copacabana",
                Numero = "1000",
                Complemento = null, // Campo opcional nulo
                Bairro = "Copacabana",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                Profissao = null, // Campo opcional nulo
                Renda = null // Campo opcional nulo
            };

            // Act
            var viewModel = LocatarioMapper.ToViewModel(dto);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(dto.Id, viewModel.Id);
            Assert.Equal(dto.Nome, viewModel.Nome);
            Assert.Equal(dto.Email, viewModel.Email);
            Assert.Equal(dto.Telefone1, viewModel.Telefone1);
            Assert.Equal(dto.Telefone2, viewModel.Telefone2);
            Assert.Equal(dto.Cpf, viewModel.Cpf);
            Assert.Equal(dto.Cep, viewModel.Cep);
            Assert.Equal(dto.Logradouro, viewModel.Logradouro);
            Assert.Equal(dto.Numero, viewModel.Numero);
            Assert.Null(viewModel.Complemento);
            Assert.Equal(dto.Bairro, viewModel.Bairro);
            Assert.Equal(dto.Cidade, viewModel.Cidade);
            Assert.Equal(dto.Estado, viewModel.Estado);
            Assert.Null(viewModel.Profissao);
            Assert.Null(viewModel.Renda);
        }

        [Fact]
        public void ToViewModel_ComValoresDecimaisPrecisao_DevePreservarCasasDecimais()
        {
            // Arrange
            var dto = new LocatarioDTO
            {
                Id = 1,
                Nome = "Carlos Pereira",
                Email = "carlos@email.com",
                Telefone1 = "31999999999",
                Telefone2 = "3133333333",
                Cpf = "11122233344",
                Cep = "30000000",
                Logradouro = "Rua da Bahia",
                Numero = "500",
                Bairro = "Centro",
                Cidade = "Belo Horizonte",
                Estado = "MG",
                Profissao = "Contador",
                Renda = 4567.89m // Valor com precisão decimal
            };

            // Act
            var viewModel = LocatarioMapper.ToViewModel(dto);

            // Assert
            Assert.Equal(4567.89m, viewModel.Renda);
        }

        #endregion

        #region ToDTO Tests

        [Fact]
        public void ToDTO_ComViewModelCompleto_DeveConverterCorretamente()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "Ana Costa",
                Email = "ana.costa@email.com",
                Telefone1 = "11987654321",
                Telefone2 = "1133445566",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua das Palmeiras",
                Numero = "456",
                Complemento = "Casa 2",
                Bairro = "Jardins",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Médica",
                Renda = 8000.75m
            };

            // Act
            var dto = LocatarioMapper.ToDTO(viewModel);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(viewModel.Id, dto.Id);
            Assert.Equal(viewModel.Nome, dto.Nome);
            Assert.Equal(viewModel.Email, dto.Email);
            Assert.Equal(viewModel.Telefone1, dto.Telefone1);
            Assert.Equal(viewModel.Telefone2, dto.Telefone2);
            Assert.Equal(viewModel.Cpf, dto.Cpf);
            Assert.Equal(viewModel.Cep, dto.Cep);
            Assert.Equal(viewModel.Logradouro, dto.Logradouro);
            Assert.Equal(viewModel.Numero, dto.Numero);
            Assert.Equal(viewModel.Complemento, dto.Complemento);
            Assert.Equal(viewModel.Bairro, dto.Bairro);
            Assert.Equal(viewModel.Cidade, dto.Cidade);
            Assert.Equal(viewModel.Estado, dto.Estado);
            Assert.Equal(viewModel.Profissao, dto.Profissao);
            Assert.Equal(viewModel.Renda, dto.Renda);
        }

        [Fact]
        public void ToDTO_ComTelefone2Nulo_DeveConverterParaStringVazia()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "Bruno Lima",
                Email = "bruno@email.com",
                Telefone1 = "21999999999",
                Telefone2 = null, // Campo opcional nulo
                Cpf = "98765432100",
                Cep = "20000000",
                Logradouro = "Av. Atlântica",
                Numero = "200",
                Bairro = "Copacabana",
                Cidade = "Rio de Janeiro",
                Estado = "RJ"
            };

            // Act
            var dto = LocatarioMapper.ToDTO(viewModel);

            // Assert
            Assert.Equal(string.Empty, dto.Telefone2);
        }

        [Fact]
        public void ToDTO_ComCamposOpcionaisNulos_DeveManterNulos()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "Teste Nulos",
                Email = "teste@email.com",
                Telefone1 = "11999999999",
                Telefone2 = null,
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Teste",
                Numero = "123",
                Complemento = null,
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = null,
                Renda = null
            };

            // Act
            var dto = LocatarioMapper.ToDTO(viewModel);

            // Assert
            Assert.Null(dto.Complemento);
            Assert.Null(dto.Profissao);
            Assert.Null(dto.Renda);
            Assert.Equal(string.Empty, dto.Telefone2); // Exceção para Telefone2
        }

        [Fact]
        public void ToDTO_ComValoresDecimaisPrecisao_DevePreservarCasasDecimais()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "Teste Decimal",
                Email = "decimal@email.com",
                Telefone1 = "11888888888",
                Telefone2 = "1122222222",
                Cpf = "99988877766",
                Cep = "87654321",
                Logradouro = "Av. Decimal",
                Numero = "999",
                Bairro = "Vila Decimal",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Testador",
                Renda = 12345.67m // Valor com precisão decimal
            };

            // Act
            var dto = LocatarioMapper.ToDTO(viewModel);

            // Assert
            Assert.Equal(12345.67m, dto.Renda);
        }

        #endregion

        #region ToViewModelList Tests

        [Fact]
        public void ToViewModelList_ComListaDTOs_DeveConverterTodosItens()
        {
            // Arrange
            var dtos = new List<LocatarioDTO>
            {
                new LocatarioDTO
                {
                    Id = 1,
                    Nome = "Locatário 1",
                    Email = "loc1@email.com",
                    Telefone1 = "11111111111",
                    Telefone2 = "1111111111",
                    Cpf = "11111111111",
                    Cep = "11111111",
                    Logradouro = "Rua 1",
                    Numero = "1",
                    Bairro = "B1",
                    Cidade = "C1",
                    Estado = "SP",
                    Profissao = "P1",
                    Renda = 1000m
                },
                new LocatarioDTO
                {
                    Id = 2,
                    Nome = "Locatário 2",
                    Email = "loc2@email.com",
                    Telefone1 = "22222222222",
                    Telefone2 = "2222222222",
                    Cpf = "22222222222",
                    Cep = "22222222",
                    Logradouro = "Rua 2",
                    Numero = "2",
                    Bairro = "B2",
                    Cidade = "C2",
                    Estado = "RJ",
                    Profissao = "P2",
                    Renda = 2000m
                }
            };

            // Act
            var viewModels = LocatarioMapper.ToViewModelList(dtos).ToList();

            // Assert
            Assert.NotNull(viewModels);
            Assert.Equal(2, viewModels.Count);

            var primeiro = viewModels.First(vm => vm.Id == 1);
            Assert.Equal("Locatário 1", primeiro.Nome);
            Assert.Equal("loc1@email.com", primeiro.Email);
            Assert.Equal("SP", primeiro.Estado);
            Assert.Equal(1000m, primeiro.Renda);

            var segundo = viewModels.First(vm => vm.Id == 2);
            Assert.Equal("Locatário 2", segundo.Nome);
            Assert.Equal("loc2@email.com", segundo.Email);
            Assert.Equal("RJ", segundo.Estado);
            Assert.Equal(2000m, segundo.Renda);
        }

        [Fact]
        public void ToViewModelList_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var dtos = new List<LocatarioDTO>();

            // Act
            var viewModels = LocatarioMapper.ToViewModelList(dtos).ToList();

            // Assert
            Assert.NotNull(viewModels);
            Assert.Empty(viewModels);
        }

        [Fact]
        public void ToViewModelList_ComDTOsComCamposNulos_DeveConverterCorretamente()
        {
            // Arrange
            var dtos = new List<LocatarioDTO>
            {
                new LocatarioDTO
                {
                    Id = 1,
                    Nome = "Teste Campos Nulos",
                    Email = "nulos@email.com",
                    Telefone1 = "11999999999",
                    Telefone2 = "1133333333",
                    Cpf = "12345678901",
                    Cep = "01234567",
                    Logradouro = "Rua Nulos",
                    Numero = "123",
                    Complemento = null,
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Profissao = null,
                    Renda = null
                }
            };

            // Act
            var viewModels = LocatarioMapper.ToViewModelList(dtos).ToList();

            // Assert
            Assert.Single(viewModels);
            var viewModel = viewModels.First();
            Assert.Equal(1, viewModel.Id);
            Assert.Equal("Teste Campos Nulos", viewModel.Nome);
            Assert.Null(viewModel.Complemento);
            Assert.Null(viewModel.Profissao);
            Assert.Null(viewModel.Renda);
        }

        #endregion

        #region Bidirectional Conversion Tests

        [Fact]
        public void ConversaoBidirecional_DTO_ViewModel_DTO_DeveManterDados()
        {
            // Arrange
            var dtoOriginal = new LocatarioDTO
            {
                Id = 1,
                Nome = "Teste Bidirecional",
                Email = "bidirecional@email.com",
                Telefone1 = "11987654321",
                Telefone2 = "1133445566",
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Bidirecional",
                Numero = "789",
                Complemento = "Bloco A",
                Bairro = "Vila Teste",
                Cidade = "São Paulo",
                Estado = "SP",
                Profissao = "Testador",
                Renda = 5555.55m
            };

            // Act
            var viewModel = LocatarioMapper.ToViewModel(dtoOriginal);
            var dtoConvertido = LocatarioMapper.ToDTO(viewModel);

            // Assert
            Assert.Equal(dtoOriginal.Id, dtoConvertido.Id);
            Assert.Equal(dtoOriginal.Nome, dtoConvertido.Nome);
            Assert.Equal(dtoOriginal.Email, dtoConvertido.Email);
            Assert.Equal(dtoOriginal.Telefone1, dtoConvertido.Telefone1);
            Assert.Equal(dtoOriginal.Telefone2, dtoConvertido.Telefone2);
            Assert.Equal(dtoOriginal.Cpf, dtoConvertido.Cpf);
            Assert.Equal(dtoOriginal.Cep, dtoConvertido.Cep);
            Assert.Equal(dtoOriginal.Logradouro, dtoConvertido.Logradouro);
            Assert.Equal(dtoOriginal.Numero, dtoConvertido.Numero);
            Assert.Equal(dtoOriginal.Complemento, dtoConvertido.Complemento);
            Assert.Equal(dtoOriginal.Bairro, dtoConvertido.Bairro);
            Assert.Equal(dtoOriginal.Cidade, dtoConvertido.Cidade);
            Assert.Equal(dtoOriginal.Estado, dtoConvertido.Estado);
            Assert.Equal(dtoOriginal.Profissao, dtoConvertido.Profissao);
            Assert.Equal(dtoOriginal.Renda, dtoConvertido.Renda);
        }

        [Fact]
        public void ConversaoBidirecional_ViewModel_DTO_ViewModel_DeveManterDados()
        {
            // Arrange
            var viewModelOriginal = new LocatarioViewModel
            {
                Id = 2,
                Nome = "Teste Reverso",
                Email = "reverso@email.com",
                Telefone1 = "21987654321",
                Telefone2 = "2133445566",
                Cpf = "98765432100",
                Cep = "20000000",
                Logradouro = "Av. Reverso",
                Numero = "987",
                Complemento = "Cobertura",
                Bairro = "Ipanema",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                Profissao = "Designer",
                Renda = 7777.77m
            };

            // Act
            var dto = LocatarioMapper.ToDTO(viewModelOriginal);
            var viewModelConvertido = LocatarioMapper.ToViewModel(dto);

            // Assert
            Assert.Equal(viewModelOriginal.Id, viewModelConvertido.Id);
            Assert.Equal(viewModelOriginal.Nome, viewModelConvertido.Nome);
            Assert.Equal(viewModelOriginal.Email, viewModelConvertido.Email);
            Assert.Equal(viewModelOriginal.Telefone1, viewModelConvertido.Telefone1);
            Assert.Equal(viewModelOriginal.Telefone2, viewModelConvertido.Telefone2);
            Assert.Equal(viewModelOriginal.Cpf, viewModelConvertido.Cpf);
            Assert.Equal(viewModelOriginal.Cep, viewModelConvertido.Cep);
            Assert.Equal(viewModelOriginal.Logradouro, viewModelConvertido.Logradouro);
            Assert.Equal(viewModelOriginal.Numero, viewModelConvertido.Numero);
            Assert.Equal(viewModelOriginal.Complemento, viewModelConvertido.Complemento);
            Assert.Equal(viewModelOriginal.Bairro, viewModelConvertido.Bairro);
            Assert.Equal(viewModelOriginal.Cidade, viewModelConvertido.Cidade);
            Assert.Equal(viewModelOriginal.Estado, viewModelConvertido.Estado);
            Assert.Equal(viewModelOriginal.Profissao, viewModelConvertido.Profissao);
            Assert.Equal(viewModelOriginal.Renda, viewModelConvertido.Renda);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void ToDTO_ComStringVaziaEmTelefone2_DeveManterStringVazia()
        {
            // Arrange
            var viewModel = new LocatarioViewModel
            {
                Id = 1,
                Nome = "Teste String Vazia",
                Email = "stringvazia@email.com",
                Telefone1 = "11999999999",
                Telefone2 = "", // String vazia
                Cpf = "12345678901",
                Cep = "01234567",
                Logradouro = "Rua Teste",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            // Act
            var dto = LocatarioMapper.ToDTO(viewModel);

            // Assert
            Assert.Equal(string.Empty, dto.Telefone2);
        }

        [Fact]
        public void ToViewModel_ComValoresLimite_DeveConverterCorretamente()
        {
            // Arrange - Valores no limite das validações
            var dto = new LocatarioDTO
            {
                Id = int.MaxValue,
                Nome = new string('A', 100), // Limite máximo
                Email = new string('b', 90) + "@teste.com",
                Telefone1 = "11999999999", // 11 dígitos
                Telefone2 = "11888888888",
                Cpf = "12345678901", // 11 dígitos
                Cep = "12345678", // 8 dígitos
                Logradouro = new string('C', 100),
                Numero = "99999", // 5 dígitos
                Complemento = new string('D', 50),
                Bairro = new string('E', 50),
                Cidade = new string('F', 45),
                Estado = "SP", // 2 dígitos
                Profissao = new string('G', 100),
                Renda = 999999.99m // Valor máximo
            };

            // Act
            var viewModel = LocatarioMapper.ToViewModel(dto);

            // Assert
            Assert.Equal(int.MaxValue, viewModel.Id);
            Assert.Equal(100, viewModel.Nome.Length);
            Assert.Equal(999999.99m, viewModel.Renda);
            Assert.Equal("SP", viewModel.Estado);
        }

        #endregion
    }
}