using Core.DTO;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Core.Service
{
    public class LocatarioService : ILocatarioService
    {
        private readonly AluguelinkContext _context;

        public LocatarioService(AluguelinkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LocatarioDTO>> GetAllAsync()
        {
            return await _context.Locatarios
                .Select(l => new LocatarioDTO
                {
                    Id = l.Id,
                    Nome = l.Nome,
                    Email = l.Email,
                    Telefone1 = l.Telefone1,
                    Telefone2 = l.Telefone2,
                    Cpf = l.Cpf,
                    Cep = l.Cep,
                    Logradouro = l.Logradouro,
                    Numero = l.Numero,
                    Complemento = l.Complemento,
                    Bairro = l.Bairro,
                    Cidade = l.Cidade,
                    Estado = l.Estado,
                    Profissao = l.Profissao,
                    Renda = l.Renda
                })
                .ToListAsync();
        }

        public async Task<LocatarioDTO?> GetByIdAsync(int id)
        {
            var locatario = await _context.Locatarios
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (locatario == null)
                return null;

            return new LocatarioDTO
            {
                Id = locatario.Id,
                Nome = locatario.Nome,
                Email = locatario.Email,
                Telefone1 = locatario.Telefone1,
                Telefone2 = locatario.Telefone2,
                Cpf = locatario.Cpf,
                Cep = locatario.Cep,
                Logradouro = locatario.Logradouro,
                Numero = locatario.Numero,
                Complemento = locatario.Complemento,
                Bairro = locatario.Bairro,
                Cidade = locatario.Cidade,
                Estado = locatario.Estado,
                Profissao = locatario.Profissao,
                Renda = locatario.Renda
            };
        }

        public async Task<LocatarioDTO> CreateAsync(LocatarioDTO locatarioDto)
        {
            var locatario = new Locatario
            {
                Nome = locatarioDto.Nome,
                Email = locatarioDto.Email,
                Telefone1 = locatarioDto.Telefone1,
                Telefone2 = locatarioDto.Telefone2,
                Cpf = locatarioDto.Cpf,
                Cep = locatarioDto.Cep,
                Logradouro = locatarioDto.Logradouro,
                Numero = locatarioDto.Numero,
                Complemento = locatarioDto.Complemento,
                Bairro = locatarioDto.Bairro,
                Cidade = locatarioDto.Cidade,
                Estado = locatarioDto.Estado,
                Profissao = locatarioDto.Profissao,
                Renda = locatarioDto.Renda
            };

            _context.Locatarios.Add(locatario);
            await _context.SaveChangesAsync();

            locatarioDto.Id = locatario.Id;
            return locatarioDto;
        }

        public async Task<LocatarioDTO?> UpdateAsync(int id, LocatarioDTO locatarioDto)
        {
            var locatario = await _context.Locatarios
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (locatario == null)
                return null;

            locatario.Nome = locatarioDto.Nome;
            locatario.Email = locatarioDto.Email;
            locatario.Telefone1 = locatarioDto.Telefone1;
            locatario.Telefone2 = locatarioDto.Telefone2;
            locatario.Cpf = locatarioDto.Cpf;
            locatario.Cep = locatarioDto.Cep;
            locatario.Logradouro = locatarioDto.Logradouro;
            locatario.Numero = locatarioDto.Numero;
            locatario.Complemento = locatarioDto.Complemento;
            locatario.Bairro = locatarioDto.Bairro;
            locatario.Cidade = locatarioDto.Cidade;
            locatario.Estado = locatarioDto.Estado;
            locatario.Profissao = locatarioDto.Profissao;
            locatario.Renda = locatarioDto.Renda;

            await _context.SaveChangesAsync();

            locatarioDto.Id = locatario.Id;
            return locatarioDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var locatario = await _context.Locatarios
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (locatario == null)
                return false;

            _context.Locatarios.Remove(locatario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}