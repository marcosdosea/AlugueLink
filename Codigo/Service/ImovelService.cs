using Core.DTO;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ImovelService : IImovelService
    {
        private readonly AluguelinkContext _context;

        public ImovelService(AluguelinkContext context)
        {
            _context = context;
        }

        private int EnsureLocador(int idLocador)
        {
            if (idLocador > 0)
            {
                if (_context.Locadors.Any(l => l.Id == idLocador))
                    return idLocador;
                throw new ServiceException($"Locador informado (ID={idLocador}) não existe.");
            }

            var existente = _context.Locadors.AsNoTracking().FirstOrDefault();
            if (existente != null)
                return existente.Id;

            var padrao = new Locador
            {
                Nome = "Locador Padrão",
                Email = $"locador.padrao@aluguelink.com",
                Telefone = "11999999999",
                Cpf = DateTime.UtcNow.Ticks.ToString().PadLeft(11, '0').Substring(0,11)
            };
            _context.Locadors.Add(padrao);
            _context.SaveChanges();
            return padrao.Id;
        }

        public int Create(Imovel imovel)
        {
            imovel.IdLocador = EnsureLocador(imovel.IdLocador);
            _context.Imovels.Add(imovel);
            _context.SaveChanges();
            return imovel.Id;
        }

        public void Edit(Imovel imovel)
        {
            imovel.IdLocador = EnsureLocador(imovel.IdLocador);
            _context.Imovels.Update(imovel);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var imovel = _context.Imovels
                .Include(i => i.Aluguels)
                .Include(i => i.Manutencaos)
                .FirstOrDefault(i => i.Id == id);

            if (imovel == null)
                return;

            var alugueisAtivos = imovel.Aluguels.Where(a => a.Status == "A").Any();
            if (alugueisAtivos)
            {
                throw new ServiceException("Não é possível excluir um imóvel que possui contratos de aluguel ativos.");
            }

            var pagamentos = _context.Pagamentos.Where(p => imovel.Aluguels.Select(a => a.Id).Contains(p.Idaluguel));
            _context.Pagamentos.RemoveRange(pagamentos);

            _context.Aluguels.RemoveRange(imovel.Aluguels);

            _context.Manutencaos.RemoveRange(imovel.Manutencaos);

            _context.Imovels.Remove(imovel);

            _context.SaveChanges();
        }

        public Imovel? Get(int id)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<Imovel> GetAll(int page, int pageSize)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .OrderBy(i => i.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IEnumerable<ImovelDto> GetByLocador(int idLocador)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .Where(i => i.IdLocador == idLocador)
                .AsNoTracking()
                .Select(i => new ImovelDto
                {
                    Id = i.Id,
                    Cep = i.Cep,
                    Logradouro = i.Logradouro,
                    Numero = i.Numero,
                    Complemento = i.Complemento,
                    Bairro = i.Bairro,
                    Cidade = i.Cidade,
                    Estado = i.Estado,
                    Tipo = i.Tipo,
                    Quartos = i.Quartos,
                    Banheiros = i.Banheiros,
                    Area = i.Area,
                    VagasGaragem = i.VagasGaragem,
                    Valor = i.Valor,
                    Descricao = i.Descricao,
                    IdLocador = i.IdLocador
                });
        }
        public IEnumerable<ImovelDto> GetByTipo(string tipo)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .Where(i => i.Tipo == tipo)
                .AsNoTracking()
                .Select(i => new ImovelDto
                {
                    Id = i.Id,
                    Cep = i.Cep,
                    Logradouro = i.Logradouro,
                    Numero = i.Numero,
                    Complemento = i.Complemento,
                    Bairro = i.Bairro,
                    Cidade = i.Cidade,
                    Estado = i.Estado,
                    Tipo = i.Tipo,
                    Quartos = i.Quartos,
                    Banheiros = i.Banheiros,
                    Area = i.Area,
                    VagasGaragem = i.VagasGaragem,
                    Valor = i.Valor,
                    Descricao = i.Descricao,
                    IdLocador = i.IdLocador
                });
        }

        public IEnumerable<ImovelDto> GetByValorRange(decimal valorMin, decimal valorMax)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .Where(i => i.Valor >= valorMin && i.Valor <= valorMax)
                .AsNoTracking()
                .Select(i => new ImovelDto
                {
                    Id = i.Id,
                    Cep = i.Cep,
                    Logradouro = i.Logradouro,
                    Numero = i.Numero,
                    Complemento = i.Complemento,
                    Bairro = i.Bairro,
                    Cidade = i.Cidade,
                    Estado = i.Estado,
                    Tipo = i.Tipo,
                    Quartos = i.Quartos,
                    Banheiros = i.Banheiros,
                    Area = i.Area,
                    VagasGaragem = i.VagasGaragem,
                    Valor = i.Valor,
                    Descricao = i.Descricao,
                    IdLocador = i.IdLocador
                });
        }
        public int GetCount()
        {
            return _context.Imovels.Count();
        }
    }
}