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

        public int Create(Locador locador)
        {
            _context.Locadors.Add(locador);
            _context.SaveChanges();
            return locador.Id;
        }

        public void Edit(Locador locador)
        {
            _context.Locadors.Update(locador);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var locador = _context.Locadors.Find(id);
            if (locador != null)
            {
                _context.Locadors.Remove(locador);
                _context.SaveChanges();
            }
        }

        public Locador? Get(int id)
        {
            return _context.Locadors.Find(id);
        }

        public Locador? GetByEmail(string email)
        {
            return _context.Locadors
                .AsNoTracking()
                .FirstOrDefault(l => l.Email == email);
        }

        public IEnumerable<Locador> GetAll(int page, int pageSize)
        {
            return _context.Locadors
                .OrderBy(l => l.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

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

        public int GetCount()
        {
            return _context.Locadors.Count();
        }
    }
}