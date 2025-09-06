using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class LocatarioService : ILocatarioService
    {
        private readonly AluguelinkContext _context;

        public LocatarioService(AluguelinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar um novo locatário na base de dados
        /// </summary>
        /// <param name="locatario">Dados do Locatário</param>
        /// <returns>ID do Locatário</returns>
        public int Create(Locatario locatario)
        {
            _context.Locatarios.Add(locatario);
            _context.SaveChanges();
            return locatario.Id;
        }

        /// <summary>
        /// Editar um locatário existente na base de dados
        /// </summary>
        /// <param name="locatario">Dados do Locatário</param>
        public void Edit(Locatario locatario)
        {
            _context.Locatarios.Update(locatario);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um locatário da base de dados
        /// </summary>
        /// <param name="id">ID do Locatário</param>
        public void Delete(int id)
        {
            var locatario = _context.Locatarios.Find(id);
            if (locatario != null)
            {
                _context.Locatarios.Remove(locatario);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Buscar um locatário na base de dados
        /// </summary>
        /// <param name="id">ID do Locatário</param>
        /// <returns>Dados do Locatário</returns>
        public Locatario? Get(int id)
        {
            return _context.Locatarios.Find(id);
        }

        /// <summary>
        /// Buscar todos os locatários na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de Locatários</returns>
        public IEnumerable<Locatario> GetAll(int page, int pageSize)
        {
            return _context.Locatarios
                .OrderBy(l => l.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar locatários por CPF
        /// </summary>
        /// <param name="cpf">CPF do locatário</param>
        /// <returns>Lista de LocatarioDTO</returns>
        public IEnumerable<LocatarioDTO> GetByCpf(string cpf)
        {
            return _context.Locatarios
                .Where(l => l.Cpf == cpf)
                .AsNoTracking()
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
                });
        }

        /// <summary>
        /// Buscar locatários por nome
        /// </summary>
        /// <param name="nome">Nome do locatário</param>
        /// <returns>Lista de LocatarioDTO</returns>
        public IEnumerable<LocatarioDTO> GetByNome(string nome)
        {
            return _context.Locatarios
                .Where(l => l.Nome.Contains(nome))
                .AsNoTracking()
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
                });
        }

        /// <summary>
        /// Contar total de locatários
        /// </summary>
        /// <returns>Número total de locatários</returns>
        public int GetCount()
        {
            return _context.Locatarios.Count();
        }
    }
}