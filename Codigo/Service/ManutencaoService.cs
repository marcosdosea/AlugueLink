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
        /// Criar uma nova manutenção na base de dados
        /// </summary>
        /// <param name="manutencao">Dados da Manutenção</param>
        /// <returns>ID da Manutenção</returns>
        public int Create(Manutencao manutencao)
        {
            _context.Manutencaos.Add(manutencao);
            _context.SaveChanges();
            return manutencao.Id;
        }

        /// <summary>
        /// Editar uma manutenção existente na base de dados
        /// </summary>
        /// <param name="manutencao">Dados da Manutenção</param>
        public void Edit(Manutencao manutencao)
        {
            _context.Manutencaos.Update(manutencao);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar uma manutenção da base de dados
        /// </summary>
        /// <param name="id">ID da Manutenção</param>
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
        /// Buscar uma manutenção na base de dados
        /// </summary>
        /// <param name="id">ID da Manutenção</param>
        /// <returns>Dados da Manutenção</returns>
        public Manutencao? Get(int id)
        {
            return _context.Manutencaos
                .Include(m => m.IdimovelNavigation)
                .FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Buscar todas as manutenções na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de Manutenções</returns>
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
        /// Buscar manutenções por imóvel
        /// </summary>
        /// <param name="idImovel">ID do imóvel</param>
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
        /// Buscar manutenções por status
        /// </summary>
        /// <param name="status">Status da manutenção</param>
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
        /// Buscar manutenções por período
        /// </summary>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
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
        /// Contar total de manutenções
        /// </summary>
        /// <returns>Número total de manutenções</returns>
        public int GetCount()
        {
            return _context.Manutencaos.Count();
        }
    }
}