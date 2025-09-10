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
        /// Criar um novo locat�rio na base de dados
        /// </summary>
        /// <param name="locatario">Dados do Locat�rio</param>
        /// <returns>ID do Locat�rio</returns>
        public int Create(Locatario locatario)
        {
            _context.Locatarios.Add(locatario);
            _context.SaveChanges();
            return locatario.Id;
        }

        /// <summary>
        /// Editar um locat�rio existente na base de dados
        /// </summary>
        /// <param name="locatario">Dados do Locat�rio</param>
        public void Edit(Locatario locatario)
        {
            _context.Locatarios.Update(locatario);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um locat�rio da base de dados
        /// </summary>
        /// <param name="id">ID do Locat�rio</param>
        public void Delete(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var locatario = _context.Locatarios
                    .Include(l => l.Aluguels)
                    .FirstOrDefault(l => l.Id == id);

                if (locatario == null)
                    return;

                // Verificar se h� alugu�is ativos
                var alugueisAtivos = locatario.Aluguels.Where(a => a.Status == "A").Any();
                if (alugueisAtivos)
                {
                    throw new InvalidOperationException("N�o � poss�vel excluir um inquilino que possui contratos de aluguel ativos.");
                }

                // Remover pagamentos relacionados aos alugu�is do locat�rio
                var pagamentos = _context.Pagamentos.Where(p => locatario.Aluguels.Select(a => a.Id).Contains(p.Idaluguel));
                _context.Pagamentos.RemoveRange(pagamentos);

                // Remover alugu�is do locat�rio
                _context.Aluguels.RemoveRange(locatario.Aluguels);

                // Remover o locat�rio
                _context.Locatarios.Remove(locatario);

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Buscar um locat�rio na base de dados
        /// </summary>
        /// <param name="id">ID do Locat�rio</param>
        /// <returns>Dados do Locat�rio</returns>
        public Locatario? Get(int id)
        {
            return _context.Locatarios.Find(id);
        }

        /// <summary>
        /// Buscar todos os locat�rios na base de dados com pagina��o
        /// </summary>
        /// <param name="page">P�gina</param>
        /// <param name="pageSize">Tamanho da p�gina</param>
        /// <returns>Lista de Locat�rios</returns>
        public IEnumerable<Locatario> GetAll(int page, int pageSize)
        {
            return _context.Locatarios
                .OrderBy(l => l.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar locat�rios por CPF
        /// </summary>
        /// <param name="cpf">CPF do locat�rio</param>
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
        /// Buscar locat�rios por nome
        /// </summary>
        /// <param name="nome">Nome do locat�rio</param>
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
        /// Contar total de locat�rios
        /// </summary>
        /// <returns>N�mero total de locat�rios</returns>
        public int GetCount()
        {
            return _context.Locatarios.Count();
        }
    }
}