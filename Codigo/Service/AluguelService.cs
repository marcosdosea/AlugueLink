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

        public int Create(Aluguel aluguel)
        {
            NormalizeStatus(aluguel);
            
            _context.Aluguels.Add(aluguel);
            _context.SaveChanges();
            return aluguel.Id;
        }

        public void Edit(Aluguel aluguel)
        {
            NormalizeStatus(aluguel);
            
            _context.Aluguels.Update(aluguel);
            _context.SaveChanges();
        }

        private void NormalizeStatus(Aluguel aluguel)
        {
            if (!string.IsNullOrEmpty(aluguel.Status))
            {
                switch (aluguel.Status.ToUpper())
                {
                    case "ATIVO":
                        aluguel.Status = "A";
                        break;
                    case "FINALIZADO":
                        aluguel.Status = "F";
                        break;
                    case "PENDENTE":
                        aluguel.Status = "P";
                        break;
                    case "A":
                    case "F": 
                    case "P":
                        break;
                    default:
                        aluguel.Status = "A";
                        break;
                }
            }
            else
            {
                aluguel.Status = "A";
            }
        }

        public void Delete(int id)
        {
            var aluguel = _context.Aluguels.Find(id);
            if (aluguel != null)
            {
                _context.Aluguels.Remove(aluguel);
                _context.SaveChanges();
            }
        }

        public Aluguel? Get(int id)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .FirstOrDefault(a => a.Id == id);
        }

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

        public IEnumerable<AluguelDto> GetByLocatario(int idLocatario)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idlocatario == idLocatario)
                .AsNoTracking()
                .Select(a => new AluguelDto
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

        public IEnumerable<AluguelDto> GetByImovel(int idImovel)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idimovel == idImovel)
                .AsNoTracking()
                .Select(a => new AluguelDto
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

        public IEnumerable<Aluguel> GetByLocador(int idLocador)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.IdimovelNavigation.IdLocador == idLocador)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<AluguelDto> GetByStatus(string status)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Status == status)
                .AsNoTracking()
                .Select(a => new AluguelDto
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

        public int GetCount()
        {
            return _context.Aluguels.Count();
        }

        public bool IsImovelAvailable(int idImovel, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null)
        {
            var query = _context.Aluguels
                .Where(a => a.Idimovel == idImovel && a.Status == "A");

            if (aluguelExcluir.HasValue)
            {
                query = query.Where(a => a.Id != aluguelExcluir.Value);
            }

            if (!dataInicio.HasValue || !dataFim.HasValue)
            {
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                return !query.Any(a => a.DataInicio <= hoje && a.DataFim >= hoje);
            }

            return !query.Any(a => dataInicio <= a.DataFim && dataFim >= a.DataInicio);
        }

        public bool IsLocatarioAvailable(int idLocatario, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null)
        {
            var query = _context.Aluguels
                .Where(a => a.Idlocatario == idLocatario && a.Status == "A");

            if (aluguelExcluir.HasValue)
            {
                query = query.Where(a => a.Id != aluguelExcluir.Value);
            }

            if (!dataInicio.HasValue || !dataFim.HasValue)
            {
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                return !query.Any(a => a.DataInicio <= hoje && a.DataFim >= hoje);
            }

            return !query.Any(a => dataInicio <= a.DataFim && dataFim >= a.DataInicio);
        }

        public IEnumerable<int> GetImoveisIndisponiveis()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Where(a => a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .Select(a => a.Idimovel)
                .Distinct()
                .ToList();
        }

        public IEnumerable<int> GetLocatariosOcupados()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Where(a => a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .Select(a => a.Idlocatario)
                .Distinct()
                .ToList();
        }

        public Aluguel? GetAluguelAtivoByImovel(int idImovel)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idimovel == idImovel && a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .FirstOrDefault();
        }

        public Aluguel? GetAluguelAtivoByLocatario(int idLocatario)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idlocatario == idLocatario && a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .FirstOrDefault();
        }

        public void AtualizarStatusAlugueis()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            var alugueis = _context.Aluguels.Where(a => 
                (a.Status == "A" && a.DataFim < hoje) || 
                (a.Status == "P" && a.DataInicio <= hoje && a.DataFim >= hoje)
            ).ToList();

            bool hasChanges = false;

            foreach (var aluguel in alugueis)
            {
                if (aluguel.Status == "A" && aluguel.DataFim < hoje)
                {
                    aluguel.Status = "F";
                    hasChanges = true;
                }
                else if (aluguel.Status == "P" && aluguel.DataInicio <= hoje && aluguel.DataFim >= hoje)
                {
                    aluguel.Status = "A";
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                _context.SaveChanges();
            }
        }
    }
}