using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class AluguelService : IAluguelService
    {
        private readonly AluguelinkContext _context;

        public AluguelService(AluguelinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar um novo aluguel na base de dados
        /// </summary>
        /// <param name="aluguel">Dados do Aluguel</param>
        /// <returns>ID do Aluguel</returns>
        public int Create(Aluguel aluguel)
        {
            _context.Aluguels.Add(aluguel);
            _context.SaveChanges();
            return aluguel.Id;
        }

        /// <summary>
        /// Editar um aluguel existente na base de dados
        /// </summary>
        /// <param name="aluguel">Dados do Aluguel</param>
        public void Edit(Aluguel aluguel)
        {
            _context.Aluguels.Update(aluguel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um aluguel da base de dados
        /// </summary>
        /// <param name="id">ID do Aluguel</param>
        public void Delete(int id)
        {
            var aluguel = _context.Aluguels.Find(id);
            if (aluguel != null)
            {
                _context.Aluguels.Remove(aluguel);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Buscar um aluguel na base de dados
        /// </summary>
        /// <param name="id">ID do Aluguel</param>
        /// <returns>Dados do Aluguel</returns>
        public Aluguel? Get(int id)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Buscar todos os aluguels na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de Aluguels</returns>
        public IEnumerable<Aluguel> GetAll(int page, int pageSize)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .OrderBy(a => a.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar aluguels por locatário
        /// </summary>
        /// <param name="idLocatario">ID do locatário</param>
        /// <returns>Lista de AluguelDTO</returns>
        public IEnumerable<AluguelDTO> GetByLocatario(int idLocatario)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idlocatario == idLocatario)
                .AsNoTracking()
                .Select(a => new AluguelDTO
                {
                    Id = a.Id,
                    DataInicio = a.DataInicio,
                    DataFim = a.DataFim,
                    Status = a.Status,
                    DataAssinatura = a.DataAssinatura,
                    Idlocatario = a.Idlocatario,
                    Idimovel = a.Idimovel
                });
        }

        /// <summary>
        /// Buscar aluguels por imóvel
        /// </summary>
        /// <param name="idImovel">ID do imóvel</param>
        /// <returns>Lista de AluguelDTO</returns>
        public IEnumerable<AluguelDTO> GetByImovel(int idImovel)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idimovel == idImovel)
                .AsNoTracking()
                .Select(a => new AluguelDTO
                {
                    Id = a.Id,
                    DataInicio = a.DataInicio,
                    DataFim = a.DataFim,
                    Status = a.Status,
                    DataAssinatura = a.DataAssinatura,
                    Idlocatario = a.Idlocatario,
                    Idimovel = a.Idimovel
                });
        }

        /// <summary>
        /// Buscar aluguels por status
        /// </summary>
        /// <param name="status">Status do aluguel</param>
        /// <returns>Lista de AluguelDTO</returns>
        public IEnumerable<AluguelDTO> GetByStatus(string status)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Status == status)
                .AsNoTracking()
                .Select(a => new AluguelDTO
                {
                    Id = a.Id,
                    DataInicio = a.DataInicio,
                    DataFim = a.DataFim,
                    Status = a.Status,
                    DataAssinatura = a.DataAssinatura,
                    Idlocatario = a.Idlocatario,
                    Idimovel = a.Idimovel
                });
        }

        /// <summary>
        /// Contar total de aluguels
        /// </summary>
        /// <returns>Número total de aluguels</returns>
        public int GetCount()
        {
            return _context.Aluguels.Count();
        }
    }
}