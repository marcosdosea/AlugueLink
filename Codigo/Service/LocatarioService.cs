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

        public int Create(Locatario locatario)
        {
            _context.Locatarios.Add(locatario);
            _context.SaveChanges();
            return locatario.Id;
        }

        public void Edit(Locatario locatario)
        {
            _context.Locatarios.Update(locatario);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var locatario = _context.Locatarios
                .Include(l => l.Aluguels)
                .FirstOrDefault(l => l.Id == id);

            if (locatario == null)
                return;

            var alugueisAtivos = locatario.Aluguels.Where(a => a.Status == "A").Any();
            if (alugueisAtivos)
            {
                throw new ServiceException("Não é possível excluir um locatário que possui contratos de aluguel ativos.");
            }

            var pagamentos = _context.Pagamentos.Where(p => locatario.Aluguels.Select(a => a.Id).Contains(p.Idaluguel));
            _context.Pagamentos.RemoveRange(pagamentos);

            _context.Aluguels.RemoveRange(locatario.Aluguels);

            _context.Locatarios.Remove(locatario);

            _context.SaveChanges();
        }

        public Locatario? Get(int id)
        {
            return _context.Locatarios.Find(id);
        }

        public IEnumerable<Locatario> GetAll(int page, int pageSize)
        {
            return _context.Locatarios
                .OrderBy(l => l.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IEnumerable<LocatarioDto> GetByCpf(string cpf)
        {
            return _context.Locatarios
                .Where(l => l.Cpf == cpf)
                .AsNoTracking()
                .Select(l => new LocatarioDto
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

        public IEnumerable<LocatarioDto> GetByNome(string nome)
        {
            return _context.Locatarios
                .Where(l => l.Nome.Contains(nome))
                .AsNoTracking()
                .Select(l => new LocatarioDto
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

        public int GetCount()
        {
            return _context.Locatarios.Count();
        }
    }
}