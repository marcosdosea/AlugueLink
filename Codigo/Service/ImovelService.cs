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
                throw new InvalidOperationException($"Locador informado (ID={idLocador}) n�o existe.");
            }

            // Tentar pegar qualquer locador existente
            var existente = _context.Locadors.AsNoTracking().FirstOrDefault();
            if (existente != null)
                return existente.Id;

            // Criar locador padr�o
            var padrao = new Locador
            {
                Nome = "Locador Padr�o",
                Email = $"locador.padrao@aluguelink.com",
                Telefone = "11999999999",
                Cpf = DateTime.UtcNow.Ticks.ToString().PadLeft(11, '0').Substring(0,11) // gera cpf dummy �nico
            };
            _context.Locadors.Add(padrao);
            _context.SaveChanges();
            return padrao.Id;
        }

        /// <summary>
        /// Criar um novo im�vel na base de dados
        /// </summary>
        /// <param name="imovel">Dados do Im�vel</param>
        /// <returns>ID do Im�vel</returns>
        public int Create(Imovel imovel)
        {
            imovel.IdLocador = EnsureLocador(imovel.IdLocador);
            _context.Imovels.Add(imovel);
            _context.SaveChanges();
            return imovel.Id;
        }

        /// <summary>
        /// Editar um im�vel existente na base de dados
        /// </summary>
        /// <param name="imovel">Dados do Im�vel</param>
        public void Edit(Imovel imovel)
        {
            imovel.IdLocador = EnsureLocador(imovel.IdLocador);
            _context.Imovels.Update(imovel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um im�vel da base de dados
        /// </summary>
        /// <param name="id">ID do Im�vel</param>
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

                // Verificar se h� alugu�is ativos
                var alugueisAtivos = imovel.Aluguels.Where(a => a.Status == "A").Any();
                if (alugueisAtivos)
                {
                    throw new InvalidOperationException("N�o � poss�vel excluir um im�vel que possui contratos de aluguel ativos.");
                }

                // Remover pagamentos relacionados aos alugu�is do im�vel
                var pagamentos = _context.Pagamentos.Where(p => imovel.Aluguels.Select(a => a.Id).Contains(p.Idaluguel));
                _context.Pagamentos.RemoveRange(pagamentos);

                // Remover alugu�is do im�vel
                _context.Aluguels.RemoveRange(imovel.Aluguels);

                // Remover manuten��es do im�vel
                _context.Manutencaos.RemoveRange(imovel.Manutencaos);

                // Remover o im�vel
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
        /// Buscar um im�vel na base de dados
        /// </summary>
        /// <param name="id">ID do Im�vel</param>
        /// <returns>Dados do Im�vel</returns>
        public Imovel? Get(int id)
        {
            return _context.Imovels
                .Include(i => i.IdLocadorNavigation)
                .FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Buscar todos os im�veis na base de dados com pagina��o
        /// </summary>
        /// <param name="page">P�gina</param>
        /// <param name="pageSize">Tamanho da p�gina</param>
        /// <returns>Lista de Im�veis</returns>
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
        /// Buscar im�veis por locador
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
        /// Buscar im�veis por tipo
        /// </summary>
        /// <param name="tipo">Tipo do im�vel</param>
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
        /// Buscar im�veis por faixa de valor
        /// </summary>
        /// <param name="valorMin">Valor m�nimo</param>
        /// <param name="valorMax">Valor m�ximo</param>
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
        /// Contar total de im�veis
        /// </summary>
        /// <returns>N�mero total de im�veis</returns>
        public int GetCount()
        {
            return _context.Imovels.Count();
        }
    }
}