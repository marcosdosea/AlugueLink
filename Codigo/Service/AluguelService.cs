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

        /// <summary>
        /// Criar um novo aluguel na base de dados
        /// </summary>
        /// <param name="aluguel">Dados do Aluguel</param>
        /// <returns>ID do Aluguel</returns>
        public int Create(Aluguel aluguel)
        {
            // Normalizar status: "Ativo" -> "A"
            NormalizeStatus(aluguel);
            
            _context.Aluguels.Add(aluguel);
            _context.SaveChanges();
            return aluguel.Id;
        }

        /// <summary>
        /// Editar um aluguel existente na base de dados
        /// </summary>
        /// <param name="aluguel">Dados do Aluguel</param>
        public void Edit(Aluguel aluguel)
        {
            // Normalizar status: "Ativo" -> "A"
            NormalizeStatus(aluguel);
            
            _context.Aluguels.Update(aluguel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Normalizar status do aluguel para garantir consist�ncia no banco
        /// </summary>
        /// <param name="aluguel">Aluguel a ser normalizado</param>
        private void NormalizeStatus(Aluguel aluguel)
        {
            if (!string.IsNullOrEmpty(aluguel.Status))
            {
                switch (aluguel.Status.ToUpper()) // Usar ToUpper para tratar c�digos tamb�m
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
                        // J� est� no formato correto, n�o precisa alterar
                        break;
                    default:
                        // Se n�o reconhecer, manter como "A" (Ativo)
                        aluguel.Status = "A";
                        break;
                }
            }
            else
            {
                // Se status vier nulo ou vazio, definir como Ativo por padr�o
                aluguel.Status = "A";
            }
        }

        /// <summary>
        /// Deletar um aluguel da base de dados
        /// </summary>
        /// <param name="id">ID do Aluguel</param>
        public void Delete(int id)
        {
            var aluguel = _context.Aluguels.Find(id);
            if (aluguel != null)
            {
                _context.Aluguels.Remove(aluguel);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Buscar um aluguel na base de dados
        /// </summary>
        /// <param name="id">ID do Aluguel</param>
        /// <returns>Dados do Aluguel</returns>
        public Aluguel? Get(int id)
        {
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Buscar todos os aluguels na base de dados com pagina��o
        /// </summary>
        /// <param name="page">P�gina</param>
        /// <param name="pageSize">Tamanho da p�gina</param>
        /// <returns>Lista de Aluguels</returns>
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

        /// <summary>
        /// Buscar alugueis por locat�rio
        /// </summary>
        /// <param name="idLocatario">ID do locat�rio</param>
        /// <returns>Lista de AluguelDto</returns>
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

        /// <summary>
        /// Buscar alugueis por im�vel
        /// </summary>
        /// <param name="idImovel">ID do im�vel</param>
        /// <returns>Lista de AluguelDto</returns>
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

        /// <summary>
        /// Buscar alugueis por status
        /// </summary>
        /// <param name="status">Status do aluguel</param>
        /// <returns>Lista de AluguelDto</returns>
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

        /// <summary>
        /// Contar total de aluguels
        /// </summary>
        /// <returns>N�mero total de aluguels</returns>
        public int GetCount()
        {
            return _context.Aluguels.Count();
        }

        /// <summary>
        /// Verificar se um im�vel est� dispon�vel para aluguel em um per�odo espec�fico
        /// </summary>
        /// <param name="idImovel">ID do im�vel</param>
        /// <param name="dataInicio">Data de in�cio do per�odo</param>
        /// <param name="dataFim">Data de fim do per�odo</param>
        /// <param name="aluguelExcluir">ID do aluguel a excluir da verifica��o (para edi��o)</param>
        /// <returns>True se dispon�vel, False se ocupado</returns>
        public bool IsImovelAvailable(int idImovel, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null)
        {
            var query = _context.Aluguels
                .Where(a => a.Idimovel == idImovel && a.Status == "A"); // Status "A" = Ativo

            if (aluguelExcluir.HasValue)
            {
                query = query.Where(a => a.Id != aluguelExcluir.Value);
            }

            // Se n�o especificar per�odo, verifica apenas alugu�is ativos atualmente
            if (!dataInicio.HasValue || !dataFim.HasValue)
            {
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                return !query.Any(a => a.DataInicio <= hoje && a.DataFim >= hoje);
            }

            // Verifica conflito de per�odo: h� sobreposi��o se:
            // (data_inicio_novo <= data_fim_existente) AND (data_fim_novo >= data_inicio_existente)
            return !query.Any(a => dataInicio <= a.DataFim && dataFim >= a.DataInicio);
        }

        /// <summary>
        /// Verificar se um locat�rio est� dispon�vel para aluguel em um per�odo espec�fico
        /// </summary>
        /// <param name="idLocatario">ID do locat�rio</param>
        /// <param name="dataInicio">Data de in�cio do per�odo</param>
        /// <param name="dataFim">Data de fim do per�odo</param>
        /// <param name="aluguelExcluir">ID do aluguel a excluir da verifica��o (para edi��o)</param>
        /// <returns>True se dispon�vel, False se ocupado</returns>
        public bool IsLocatarioAvailable(int idLocatario, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null)
        {
            var query = _context.Aluguels
                .Where(a => a.Idlocatario == idLocatario && a.Status == "A"); // Status "A" = Ativo

            if (aluguelExcluir.HasValue)
            {
                query = query.Where(a => a.Id != aluguelExcluir.Value);
            }

            // Se n�o especificar per�odo, verifica apenas alugu�is ativos atualmente
            if (!dataInicio.HasValue || !dataFim.HasValue)
            {
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                return !query.Any(a => a.DataInicio <= hoje && a.DataFim >= hoje);
            }

            // Verifica conflito de per�odo
            return !query.Any(a => dataInicio <= a.DataFim && dataFim >= a.DataInicio);
        }

        /// <summary>
        /// Obter lista de IDs de im�veis que est�o indispon�veis (com aluguel ativo)
        /// </summary>
        /// <returns>Lista de IDs de im�veis ocupados</returns>
        public IEnumerable<int> GetImoveisIndisponiveis()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Where(a => a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .Select(a => a.Idimovel)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Obter lista de IDs de locat�rios que est�o ocupados (com aluguel ativo)
        /// </summary>
        /// <returns>Lista de IDs de locat�rios ocupados</returns>
        public IEnumerable<int> GetLocatariosOcupados()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Where(a => a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .Select(a => a.Idlocatario)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Obter aluguel ativo de um im�vel
        /// </summary>
        /// <param name="idImovel">ID do im�vel</param>
        /// <returns>Aluguel ativo ou null</returns>
        public Aluguel? GetAluguelAtivoByImovel(int idImovel)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idimovel == idImovel && a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .FirstOrDefault();
        }

        /// <summary>
        /// Obter aluguel ativo de um locat�rio
        /// </summary>
        /// <param name="idLocatario">ID do locat�rio</param>
        /// <returns>Aluguel ativo ou null</returns>
        public Aluguel? GetAluguelAtivoByLocatario(int idLocatario)
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            return _context.Aluguels
                .Include(a => a.IdlocatarioNavigation)
                .Include(a => a.IdimovelNavigation)
                .Where(a => a.Idlocatario == idLocatario && a.Status == "A" && a.DataInicio <= hoje && a.DataFim >= hoje)
                .FirstOrDefault();
        }

        /// <summary>
        /// Atualizar status dos alugu�is baseado nas datas atuais
        /// Este m�todo pode ser chamado periodicamente para manter o status consistente
        /// </summary>
        public void AtualizarStatusAlugueis()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            // Buscar todos os alugu�is que precisam ter status atualizado
            var alugueis = _context.Aluguels.Where(a => 
                (a.Status == "A" && a.DataFim < hoje) || // Alugu�is ativos que j� expiraram
                (a.Status == "P" && a.DataInicio <= hoje && a.DataFim >= hoje) // Alugu�is pendentes que j� come�aram
            ).ToList();

            bool hasChanges = false;

            foreach (var aluguel in alugueis)
            {
                if (aluguel.Status == "A" && aluguel.DataFim < hoje)
                {
                    // Aluguel expirado - marcar como finalizado
                    aluguel.Status = "F";
                    hasChanges = true;
                }
                else if (aluguel.Status == "P" && aluguel.DataInicio <= hoje && aluguel.DataFim >= hoje)
                {
                    // Aluguel pendente que j� come�ou - marcar como ativo
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