using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PagamentoService : IPagamentoService
    {
        private readonly AluguelinkContext _context;

        public PagamentoService(AluguelinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar um novo pagamento na base de dados
        /// </summary>
        /// <param name="pagamento">Dados do Pagamento</param>
        /// <returns>ID do Pagamento</returns>
        public int Create(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
            _context.SaveChanges();
            return pagamento.Id;
        }

        /// <summary>
        /// Editar um pagamento existente na base de dados
        /// </summary>
        /// <param name="pagamento">Dados do Pagamento</param>
        public void Edit(Pagamento pagamento)
        {
            _context.Pagamentos.Update(pagamento);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um pagamento da base de dados
        /// </summary>
        /// <param name="id">ID do Pagamento</param>
        public void Delete(int id)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento != null)
            {
                _context.Pagamentos.Remove(pagamento);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Buscar um pagamento na base de dados
        /// </summary>
        /// <param name="id">ID do Pagamento</param>
        /// <returns>Dados do Pagamento</returns>
        public Pagamento? Get(int id)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Buscar todos os pagamentos na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de Pagamentos</returns>
        public IEnumerable<Pagamento> GetAll(int page, int pageSize)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .OrderBy(p => p.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar pagamentos por aluguel
        /// </summary>
        /// <param name="idAluguel">ID do aluguel</param>
        /// <returns>Lista de PagamentoDTO</returns>
        public IEnumerable<PagamentoDTO> GetByAluguel(int idAluguel)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .Where(p => p.Idaluguel == idAluguel)
                .AsNoTracking()
                .Select(p => new PagamentoDTO
                {
                    Id = p.Id,
                    DataPagamento = p.DataPagamento,
                    TipoPagamento = p.TipoPagamento,
                    Valor = p.Valor,
                    Idaluguel = p.Idaluguel
                });
        }

        /// <summary>
        /// Buscar pagamentos por tipo de pagamento
        /// </summary>
        /// <param name="tipoPagamento">Tipo do pagamento</param>
        /// <returns>Lista de PagamentoDTO</returns>
        public IEnumerable<PagamentoDTO> GetByTipoPagamento(string tipoPagamento)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .Where(p => p.TipoPagamento == tipoPagamento)
                .AsNoTracking()
                .Select(p => new PagamentoDTO
                {
                    Id = p.Id,
                    DataPagamento = p.DataPagamento,
                    TipoPagamento = p.TipoPagamento,
                    Valor = p.Valor,
                    Idaluguel = p.Idaluguel
                });
        }

        /// <summary>
        /// Buscar pagamentos por período
        /// </summary>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
        /// <returns>Lista de PagamentoDTO</returns>
        public IEnumerable<PagamentoDTO> GetByPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .Where(p => p.DataPagamento >= dataInicio && p.DataPagamento <= dataFim)
                .AsNoTracking()
                .Select(p => new PagamentoDTO
                {
                    Id = p.Id,
                    DataPagamento = p.DataPagamento,
                    TipoPagamento = p.TipoPagamento,
                    Valor = p.Valor,
                    Idaluguel = p.Idaluguel
                });
        }

        /// <summary>
        /// Contar total de pagamentos
        /// </summary>
        /// <returns>Número total de pagamentos</returns>
        public int GetCount()
        {
            return _context.Pagamentos.Count();
        }
    }
}