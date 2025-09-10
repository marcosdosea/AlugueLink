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
            // Se veio um Id informado e existe, retorna
            if (idLocador > 0)
            {
                if (_context.Locadors.Any(l => l.Id == idLocador))
                    return idLocador;
                throw new InvalidOperationException($"Locador informado (ID={idLocador}) não existe.");
            }

            // Tentar pegar qualquer locador existente
            var existente = _context.Locadors.AsNoTracking().FirstOrDefault();
            if (existente != null)
                return existente.Id;

            // Criar locador padrão
            var padrao = new Locador
            {
                Nome = "Locador Padrão",
                Email = $"locador.padrao@aluguelink.com",
                Telefone = "11999999999",
                Cpf = DateTime.UtcNow.Ticks.ToString().PadLeft(11, '0').Substring(0,11) // gera cpf dummy único
            };
            _context.Locadors.Add(padrao);
            _context.SaveChanges();
            return padrao.Id;
        }

        /// <summary>
        /// Criar um novo imóvel na base de dados
        /// </summary>
        /// <param name="imovel">Dados do Imóvel</param>
        /// <returns>ID do Imóvel</returns>
        public int Create(Imovel imovel)
        {
            imovel.IdLocador = EnsureLocador(imovel.IdLocador);
            _context.Imovels.Add(imovel);
            _context.SaveChanges();
            return imovel.Id;
        }

        /// <summary>
        /// Editar um imóvel existente na base de dados
        /// </summary>
        /// <param name="imovel">Dados do Imóvel</param>
        public void Edit(Imovel imovel)
        {
            imovel.IdLocador = EnsureLocador(imovel.IdLocador);
            _context.Imovels.Update(imovel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um imóvel da base de dados
        /// </summary>
        /// <param name="id">ID do Imóvel</param>
        public void Delete(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var imovel = _context.Imovels
                    .Include(i => i.Aluguels)
                    .Include(i => i.Manutencaos)
                    .FirstOrDefault(i => i.Id == id);

                if (imovel == null)
                    return;

                // Verificar se há aluguéis ativos
                var alugueisAtivos = imovel.Aluguels.Where(a => a.Status == "A").Any();
                if (alugueisAtivos)
                {
                    throw new InvalidOperationException("Não é possível excluir um imóvel que possui contratos de aluguel ativos.");
                }

                // Remover pagamentos relacionados aos aluguéis do imóvel
                var pagamentos = _context.Pagamentos.Where(p => imovel.Aluguels.Select(a => a.Id).Contains(p.Idaluguel));
                _context.Pagamentos.RemoveRange(pagamentos);

                // Remover aluguéis do imóvel
                _context.Aluguels.RemoveRange(imovel.Aluguels);

                // Remover manutenções do imóvel
                _context.Manutencaos.RemoveRange(imovel.Manutencaos);

                // Remover o imóvel
                _context.Imovels.Remove(imovel);

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Buscar um imóvel na base de dados
        /// </summary>
        /// <param name="id">ID do Imóvel</param>
        /// <returns>Dados do Imóvel</returns>
        public Imovel? Get(int id)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Buscar todos os imóveis na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de Imóveis</returns>
        public IEnumerable<Imovel> GetAll(int page, int pageSize)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .OrderBy(i => i.Id)
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Buscar imóveis por locador
        /// </summary>
        /// <param name="idLocador">ID do locador</param>
        /// <returns>Lista de ImovelDTO</returns>
        public IEnumerable<ImovelDTO> GetByLocador(int idLocador)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .Where(i => i.IdLocador == idLocador)
                .AsNoTracking()
                .Select(i => new ImovelDTO
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

        /// <summary>
        /// Buscar imóveis por tipo
        /// </summary>
        /// <param name="tipo">Tipo do imóvel</param>
        /// <returns>Lista de ImovelDTO</returns>
        public IEnumerable<ImovelDTO> GetByTipo(string tipo)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .Where(i => i.Tipo == tipo)
                .AsNoTracking()
                .Select(i => new ImovelDTO
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

        /// <summary>
        /// Buscar imóveis por faixa de valor
        /// </summary>
        /// <param name="valorMin">Valor mínimo</param>
        /// <param name="valorMax">Valor máximo</param>
        /// <returns>Lista de ImovelDTO</returns>
        public IEnumerable<ImovelDTO> GetByValorRange(decimal valorMin, decimal valorMax)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .Where(i => i.Valor >= valorMin && i.Valor <= valorMax)
                .AsNoTracking()
                .Select(i => new ImovelDTO
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

        /// <summary>
        /// Contar total de imóveis
        /// </summary>
        /// <returns>Número total de imóveis</returns>
        public int GetCount()
        {
            return _context.Imovels.Count();
        }
    }
}