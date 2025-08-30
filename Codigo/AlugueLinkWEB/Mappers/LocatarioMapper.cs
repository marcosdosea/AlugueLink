using AlugueLinkWEB.Models;
using Core.DTO;

namespace AlugueLinkWEB.Mappers
{
    public static class LocatarioMapper
    {
        public static LocatarioViewModel ToViewModel(LocatarioDTO dto)
        {
            return new LocatarioViewModel
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone1 = dto.Telefone1,
                Telefone2 = dto.Telefone2,
                Cpf = dto.Cpf,
                Cep = dto.Cep,
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Profissao = dto.Profissao,
                Renda = dto.Renda
            };
        }

        public static LocatarioDTO ToDTO(LocatarioViewModel viewModel)
        {
            return new LocatarioDTO
            {
                Id = viewModel.Id,
                Nome = viewModel.Nome,
                Email = viewModel.Email,
                Telefone1 = viewModel.Telefone1,
                Telefone2 = viewModel.Telefone2 ?? string.Empty,
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
        }

        public static IEnumerable<LocatarioViewModel> ToViewModelList(IEnumerable<LocatarioDTO> dtos)
        {
            return dtos.Select(ToViewModel);
        }
    }
}