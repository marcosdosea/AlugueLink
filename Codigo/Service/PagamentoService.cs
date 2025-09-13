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

        public int Create(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
            _context.SaveChanges();
            return pagamento.Id;
        }

        public void Edit(Pagamento pagamento)
        {
            try
            {
                var tracked = _context.ChangeTracker.Entries<Pagamento>()
                    .FirstOrDefault(e => e.Entity.Id == pagamento.Id);
                
                if (tracked != null)
                {
                    tracked.Entity.Valor = pagamento.Valor;
                    tracked.Entity.DataPagamento = pagamento.DataPagamento;
                    tracked.Entity.TipoPagamento = pagamento.TipoPagamento;
                    tracked.Entity.Idaluguel = pagamento.Idaluguel;
                }
                else
                {
                    _context.Pagamentos.Attach(pagamento);
                    _context.Entry(pagamento).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                
                _context.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                var existingPagamento = _context.Pagamentos
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == pagamento.Id);
                
                if (existingPagamento != null)
                {
                    _context.Update(pagamento);
                    _context.SaveChanges();
                }
            }
        }

        public void Delete(int id)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento != null)
            {
                _context.Pagamentos.Remove(pagamento);
                _context.SaveChanges();
            }
        }

        public Pagamento? Get(int id)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                    .ThenInclude(a => a.IdimovelNavigation)
                .Include(p => p.IdaluguelNavigation)
                    .ThenInclude(a => a.IdlocatarioNavigation)
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Pagamento> GetAll(int page, int pageSize)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .OrderBy(p => p.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IEnumerable<Pagamento> GetByLocador(int locadorId, int page, int pageSize)
        {
            try
            {
                var query = from p in _context.Pagamentos
                           join a in _context.Aluguels on p.Idaluguel equals a.Id into alugueis
                           from al in alugueis.DefaultIfEmpty()
                           join i in _context.Imovels on (al != null ? al.Idimovel : 0) equals i.Id into imoveis
                           from im in imoveis.DefaultIfEmpty()
                           where im != null && im.IdLocador == locadorId
                           orderby p.DataPagamento descending
                           select p;

                var pagamentos = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var pagamento in pagamentos)
                {
                    _context.Entry(pagamento)
                        .Reference(p => p.IdaluguelNavigation)
                        .Load();

                    if (pagamento.IdaluguelNavigation != null)
                    {
                        _context.Entry(pagamento.IdaluguelNavigation)
                            .Reference(a => a.IdimovelNavigation)
                            .Load();

                        _context.Entry(pagamento.IdaluguelNavigation)
                            .Reference(a => a.IdlocatarioNavigation)
                            .Load();
                    }
                }

                return pagamentos;
            }
            catch
            {
                try
                {
                    return _context.Pagamentos
                        .Where(p => _context.Aluguels
                            .Any(a => a.Id == p.Idaluguel && 
                                     _context.Imovels.Any(i => i.Id == a.Idimovel && i.IdLocador == locadorId)))
                        .OrderByDescending(p => p.DataPagamento)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }
                catch
                {
                    return new List<Pagamento>();
                }
            }
        }

        public int GetCountByLocador(int locadorId)
        {
            try
            {
                return _context.Pagamentos
                    .Where(p => _context.Aluguels
                        .Any(a => a.Id == p.Idaluguel && 
                                 _context.Imovels.Any(i => i.Id == a.Idimovel && i.IdLocador == locadorId)))
                    .Count();
            }
            catch
            {
                return 0;
            }
        }

        public IEnumerable<PagamentoDto> GetByAluguel(int idAluguel)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .Where(p => p.Idaluguel == idAluguel)
                .AsNoTracking()
                .Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    DataPagamento = p.DataPagamento,
                    TipoPagamento = p.TipoPagamento,
                    Valor = p.Valor,
                    Idaluguel = p.Idaluguel
                });
        }

        public IEnumerable<PagamentoDto> GetByTipoPagamento(string tipoPagamento)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .Where(p => p.TipoPagamento == tipoPagamento)
                .AsNoTracking()
                .Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    DataPagamento = p.DataPagamento,
                    TipoPagamento = p.TipoPagamento,
                    Valor = p.Valor,
                    Idaluguel = p.Idaluguel
                });
        }

        public IEnumerable<PagamentoDto> GetByPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return _context.Pagamentos
                .Include(p => p.IdaluguelNavigation)
                .Where(p => p.DataPagamento >= dataInicio && p.DataPagamento <= dataFim)
                .AsNoTracking()
                .Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    DataPagamento = p.DataPagamento,
                    TipoPagamento = p.TipoPagamento,
                    Valor = p.Valor,
                    Idaluguel = p.Idaluguel
                });
        }

        public int GetCount()
        {
            return _context.Pagamentos.Count();
        }
    }
}