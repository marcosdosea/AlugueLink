using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ManutencaoService : IManutencaoService
    {
        private readonly AluguelinkContext _context;

        public ManutencaoService(AluguelinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar uma nova manuten��o na base de dados
        /// </summary>
        /// <param name="manutencao">Dados da Manuten��o</param>
        /// <returns>ID da Manuten��o</returns>
        public int Create(Manutencao manutencao)
        {
            _context.Manutencaos.Add(manutencao);
            _context.SaveChanges();
            return manutencao.Id;
        }

        /// <summary>
        /// Editar uma manuten��o existente na base de dados
        /// </summary>
        /// <param name="manutencao">Dados da Manuten��o</param>
        public void Edit(Manutencao manutencao)
        {
            _context.Manutencaos.Update(manutencao);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar uma manuten��o da base de dados
        /// </summary>
        /// <param name="id">ID da Manuten��o</param>
        public void Delete(int id)
        {
            var manutencao = _context.Manutencaos.Find(id);
            if (manutencao != null)
            {
                _context.Manutencaos.Remove(manutencao);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Buscar uma manuten��o na base de dados
        /// </summary>
        /// <param name="id">ID da Manuten��o</param>
        /// <returns>Dados da Manuten��o</returns>
        public Manutencao? Get(int id)
        {
            return _context.Manutencaos
                .Include(m => m.IdimovelNavigation)
                .FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Buscar todas as manuten��es na base de dados com pagina��o
        /// </summary>
        /// <param name="page">P�gina</param>
        /// <param name="pageSize">Tamanho da p�gina</param>
        /// <returns>Lista de Manuten��es</returns>
        public IEnumerable<Manutencao> GetAll(int page, int pageSize)
        {
            return _context.Manutencaos
                .Include(m => m.IdimovelNavigation)
                .OrderBy(m => m.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar manuten��es por im�vel
        /// </summary>
        /// <param name="idImovel">ID do im�vel</param>
        /// <returns>Lista de ManutencaoDTO</returns>
        public IEnumerable<ManutencaoDTO> GetByImovel(int idImovel)
        {
            return _context.Manutencaos
                .Include(m => m.IdimovelNavigation)
                .Where(m => m.Idimovel == idImovel)
                .AsNoTracking()
                .Select(m => new ManutencaoDTO
                {
                    Id = m.Id,
                    DataSolicitacao = m.DataSolicitacao,
                    Descricao = m.Descricao,
                    Status = m.Status,
                    Valor = m.Valor,
                    Idimovel = m.Idimovel
                });
        }

        /// <summary>
        /// Buscar manuten��es por status
        /// </summary>
        /// <param name="status">Status da manuten��o</param>
        /// <returns>Lista de ManutencaoDTO</returns>
        public IEnumerable<ManutencaoDTO> GetByStatus(string status)
        {
            return _context.Manutencaos
                .Include(m => m.IdimovelNavigation)
                .Where(m => m.Status == status)
                .AsNoTracking()
                .Select(m => new ManutencaoDTO
                {
                    Id = m.Id,
                    DataSolicitacao = m.DataSolicitacao,
                    Descricao = m.Descricao,
                    Status = m.Status,
                    Valor = m.Valor,
                    Idimovel = m.Idimovel
                });
        }

        /// <summary>
        /// Buscar manuten��es por per�odo
        /// </summary>
        /// <param name="dataInicio">Data de in�cio do per�odo</param>
        /// <param name="dataFim">Data de fim do per�odo</param>
        /// <returns>Lista de ManutencaoDTO</returns>
        public IEnumerable<ManutencaoDTO> GetByPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return _context.Manutencaos
                .Include(m => m.IdimovelNavigation)
                .Where(m => m.DataSolicitacao >= dataInicio && m.DataSolicitacao <= dataFim)
                .AsNoTracking()
                .Select(m => new ManutencaoDTO
                {
                    Id = m.Id,
                    DataSolicitacao = m.DataSolicitacao,
                    Descricao = m.Descricao,
                    Status = m.Status,
                    Valor = m.Valor,
                    Idimovel = m.Idimovel
                });
        }

        /// <summary>
        /// Contar total de manuten��es
        /// </summary>
        /// <returns>N�mero total de manuten��es</returns>
        public int GetCount()
        {
            return _context.Manutencaos.Count();
        }
    }
}