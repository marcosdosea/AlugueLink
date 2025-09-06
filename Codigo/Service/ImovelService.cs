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

        /// <summary>
        /// Criar um novo im�vel na base de dados
        /// </summary>
        /// <param name="imovel">Dados do Im�vel</param>
        /// <returns>ID do Im�vel</returns>
        public int Create(Imovel imovel)
        {
            // Se nenhum locador foi informado (0), atribui locador padr�o (ex: 1)
            if (imovel.IdLocador == 0)
            {
                // Verificar se locador padr�o existe; se n�o, criar um m�nimo
                var padrao = _context.Locadors.FirstOrDefault(l => l.Id == 1);
                if (padrao == null)
                {
                    padrao = new Locador
                    {
                        Id = 1,
                        Nome = "Locador Padr�o",
                        Email = "locador@padrao.com",
                        Telefone = "0000000000",
                        Cpf = "00000000000"
                    };
                    _context.Locadors.Add(padrao);
                    _context.SaveChanges();
                }
                imovel.IdLocador = padrao.Id;
            }
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
            if (imovel.IdLocador == 0)
            {
                var padrao = _context.Locadors.FirstOrDefault(l => l.Id == 1) ?? new Locador
                {
                    Id = 1,
                    Nome = "Locador Padr�o",
                    Email = "locador@padrao.com",
                    Telefone = "0000000000",
                    Cpf = "00000000000"
                };
                if (padrao.Id == 0)
                {
                    _context.Locadors.Add(padrao);
                    _context.SaveChanges();
                }
                imovel.IdLocador = padrao.Id;
            }
            _context.Imovels.Update(imovel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletar um im�vel da base de dados
        /// </summary>
        /// <param name="id">ID do Im�vel</param>
        public void Delete(int id)
        {
            var imovel = _context.Imovels.Find(id);
            if (imovel != null)
            {
                _context.Imovels.Remove(imovel);
                _context.SaveChanges();
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