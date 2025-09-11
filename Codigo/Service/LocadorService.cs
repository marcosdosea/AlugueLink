using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class LocadorService : ILocadorService
    {
        private readonly AluguelinkContext _context;

        public LocadorService(AluguelinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar um novo locador na base de dados
        /// </summary>
        /// <param name="locador">Dados do Locador</param>
        /// <returns>ID do Locador</returns>
        public int Create(Locador locador)
        {
            _context.Locadors.Add(locador);
            _context.SaveChanges();
            return locador.Id;
        }

        /// <summary>
        /// Editar um locador existente na base de dados
        /// </summary>
        /// <param name="locador">Dados do Locador</param>
        public void Edit(Locador locador)
        {
            _context.Locadors.Update(locador);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um locador da base de dados
        /// </summary>
        /// <param name="id">ID do Locador</param>
        public void Delete(int id)
        {
            var locador = _context.Locadors.Find(id);
            if (locador != null)
            {
                _context.Locadors.Remove(locador);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Buscar um locador na base de dados
        /// </summary>
        /// <param name="id">ID do Locador</param>
        /// <returns>Dados do Locador</returns>
        public Locador? Get(int id)
        {
            return _context.Locadors.Find(id);
        }

        /// <summary>
        /// Buscar todos os locadores na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de Locadores</returns>
        public IEnumerable<Locador> GetAll(int page, int pageSize)
        {
            return _context.Locadors
                .OrderBy(l => l.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar locadores por CPF
        /// </summary>
        /// <param name="cpf">CPF do locador</param>
        /// <returns>Lista de LocadorDto</returns>
        public IEnumerable<LocadorDto> GetByCpf(string cpf)
        {
            return _context.Locadors
                .Where(l => l.Cpf == cpf)
                .AsNoTracking()
                .Select(l => new LocadorDto
                {
                    Id = l.Id,
                    Nome = l.Nome,
                    Email = l.Email,
                    Telefone = l.Telefone,
                    Cpf = l.Cpf
                });
        }

        /// <summary>
        /// Buscar locadores por nome
        /// </summary>
        /// <param name="nome">Nome do locador</param>
        /// <returns>Lista de LocadorDto</returns>
        public IEnumerable<LocadorDto> GetByNome(string nome)
        {
            return _context.Locadors
                .Where(l => l.Nome.Contains(nome))
                .AsNoTracking()
                .Select(l => new LocadorDto
                {
                    Id = l.Id,
                    Nome = l.Nome,
                    Email = l.Email,
                    Telefone = l.Telefone,
                    Cpf = l.Cpf
                });
        }

        /// <summary>
        /// Contar total de locadores
        /// </summary>
        /// <returns>Número total de locadores</returns>
        public int GetCount()
        {
            return _context.Locadors.Count();
        }
    }
}