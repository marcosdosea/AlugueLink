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
        /// Normalizar status do aluguel para garantir consistência no banco
        /// </summary>
        /// <param name="aluguel">Aluguel a ser normalizado</param>
        private void NormalizeStatus(Aluguel aluguel)
        {
            if (!string.IsNullOrEmpty(aluguel.Status))
            {
                switch (aluguel.Status.ToUpper()) // Usar ToUpper para tratar códigos também
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
                        // Já está no formato correto, não precisa alterar
                        break;
                    default:
                        // Se não reconhecer, manter como "A" (Ativo)
                        aluguel.Status = "A";
                        break;
                }
            }
            else
            {
                // Se status vier nulo ou vazio, definir como Ativo por padrão
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
        /// Buscar todos os aluguels na base de dados com paginação
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
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
        /// Buscar alugueis por locatário
        /// </summary>
        /// <param name="idLocatario">ID do locatário</param>
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
        /// Buscar alugueis por imóvel
        /// </summary>
        /// <param name="idImovel">ID do imóvel</param>
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
        /// <returns>Número total de aluguels</returns>
        public int GetCount()
        {
            return _context.Aluguels.Count();
        }

        /// <summary>
        /// Verificar se um imóvel está disponível para aluguel em um período específico
        /// </summary>
        /// <param name="idImovel">ID do imóvel</param>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
        /// <param name="aluguelExcluir">ID do aluguel a excluir da verificação (para edição)</param>
        /// <returns>True se disponível, False se ocupado</returns>
        public bool IsImovelAvailable(int idImovel, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null)
        {
            var query = _context.Aluguels
                .Where(a => a.Idimovel == idImovel && a.Status == "A"); // Status "A" = Ativo

            if (aluguelExcluir.HasValue)
            {
                query = query.Where(a => a.Id != aluguelExcluir.Value);
            }

            // Se não especificar período, verifica apenas aluguéis ativos atualmente
            if (!dataInicio.HasValue || !dataFim.HasValue)
            {
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                return !query.Any(a => a.DataInicio <= hoje && a.DataFim >= hoje);
            }

            // Verifica conflito de período: há sobreposição se:
            // (data_inicio_novo <= data_fim_existente) AND (data_fim_novo >= data_inicio_existente)
            return !query.Any(a => dataInicio <= a.DataFim && dataFim >= a.DataInicio);
        }

        /// <summary>
        /// Verificar se um locatário está disponível para aluguel em um período específico
        /// </summary>
        /// <param name="idLocatario">ID do locatário</param>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
        /// <param name="aluguelExcluir">ID do aluguel a excluir da verificação (para edição)</param>
        /// <returns>True se disponível, False se ocupado</returns>
        public bool IsLocatarioAvailable(int idLocatario, DateOnly? dataInicio = null, DateOnly? dataFim = null, int? aluguelExcluir = null)
        {
            var query = _context.Aluguels
                .Where(a => a.Idlocatario == idLocatario && a.Status == "A"); // Status "A" = Ativo

            if (aluguelExcluir.HasValue)
            {
                query = query.Where(a => a.Id != aluguelExcluir.Value);
            }

            // Se não especificar período, verifica apenas aluguéis ativos atualmente
            if (!dataInicio.HasValue || !dataFim.HasValue)
            {
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                return !query.Any(a => a.DataInicio <= hoje && a.DataFim >= hoje);
            }

            // Verifica conflito de período
            return !query.Any(a => dataInicio <= a.DataFim && dataFim >= a.DataInicio);
        }

        /// <summary>
        /// Obter lista de IDs de imóveis que estão indisponíveis (com aluguel ativo)
        /// </summary>
        /// <returns>Lista de IDs de imóveis ocupados</returns>
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
        /// Obter lista de IDs de locatários que estão ocupados (com aluguel ativo)
        /// </summary>
        /// <returns>Lista de IDs de locatários ocupados</returns>
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
        /// Obter aluguel ativo de um imóvel
        /// </summary>
        /// <param name="idImovel">ID do imóvel</param>
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
        /// Obter aluguel ativo de um locatário
        /// </summary>
        /// <param name="idLocatario">ID do locatário</param>
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
        /// Atualizar status dos aluguéis baseado nas datas atuais
        /// Este método pode ser chamado periodicamente para manter o status consistente
        /// </summary>
        public void AtualizarStatusAlugueis()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            
            // Buscar todos os aluguéis que precisam ter status atualizado
            var alugueis = _context.Aluguels.Where(a => 
                (a.Status == "A" && a.DataFim < hoje) || // Aluguéis ativos que já expiraram
                (a.Status == "P" && a.DataInicio <= hoje && a.DataFim >= hoje) // Aluguéis pendentes que já começaram
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
                    // Aluguel pendente que já começou - marcar como ativo
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