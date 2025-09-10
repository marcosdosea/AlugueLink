using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers
{
    [Authorize]
    public class ImovelController : Controller
    {
        private readonly IImovelService imovelService;
        private readonly ILocadorService locadorService;
        private readonly IAluguelService aluguelService;
        private readonly IMapper mapper;

        public ImovelController(IImovelService imovelService, ILocadorService locadorService, 
            IAluguelService aluguelService, IMapper mapper)
        {
            this.imovelService = imovelService;
            this.locadorService = locadorService;
            this.aluguelService = aluguelService;
            this.mapper = mapper;
        }

        // GET: Imovel
        public IActionResult Index(int page = 1, int pageSize = 10, string? filtro = "todos")
        {
            try
            {
                // Atualizar status dos alugu�is antes de verificar disponibilidade
                aluguelService.AtualizarStatusAlugueis();
            }
            catch (Exception)
            {
                // Continua mesmo se houver erro na atualiza��o
            }

            var imoveis = imovelService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<ImovelViewModel>>(imoveis).ToList();

            // Informa��es de status de aluguel
            var imoveisIndisponiveis = aluguelService.GetImoveisIndisponiveis().ToList();
            foreach (var viewModel in viewModels)
            {
                viewModel.IsAlugado = imoveisIndisponiveis.Contains(viewModel.Id);
                if (viewModel.IsAlugado)
                {
                    var aluguelAtivo = aluguelService.GetAluguelAtivoByImovel(viewModel.Id);
                    if (aluguelAtivo != null)
                    {
                        viewModel.InquilinoAtual = aluguelAtivo.IdlocatarioNavigation?.Nome;
                        viewModel.DataInicioAluguel = aluguelAtivo.DataInicio;
                        viewModel.DataFimAluguel = aluguelAtivo.DataFim;
                    }
                }
            }

            // Aplicar filtro
            filtro = (filtro ?? "todos").ToLowerInvariant();
            IEnumerable<ImovelViewModel> filtrados = viewModels;
            switch (filtro)
            {
                case "alugados":
                    filtrados = viewModels.Where(vm => vm.IsAlugado);
                    break;
                case "disponiveis":
                case "dispon�veis":
                    filtrados = viewModels.Where(vm => !vm.IsAlugado);
                    break;
                default:
                    filtrados = viewModels;
                    break;
            }

            ViewBag.Filtro = filtro;
            ViewBag.TotalItems = imovelService.GetCount();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(filtrados);
        }

        // GET: Imovel/Details/5
        public IActionResult Details(int id)
        {
            var imovel = imovelService.Get(id);
            if (imovel == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<ImovelViewModel>(imovel);
            
            // Adicionar informa��es de status de aluguel
            viewModel.IsAlugado = !aluguelService.IsImovelAvailable(id);
            if (viewModel.IsAlugado)
            {
                var aluguelAtivo = aluguelService.GetAluguelAtivoByImovel(id);
                if (aluguelAtivo != null)
                {
                    viewModel.InquilinoAtual = aluguelAtivo.IdlocatarioNavigation?.Nome;
                    viewModel.DataInicioAluguel = aluguelAtivo.DataInicio;
                    viewModel.DataFimAluguel = aluguelAtivo.DataFim;
                }
            }
            
            return View(viewModel);
        }

        // GET: Imovel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Imovel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ImovelViewModel viewModel)
        {
            // Valida��o condicional para tipo comercial: quartos/banheiros n�o obrigat�rios
            if (!viewModel.IsComercial)
            {
                if (!viewModel.Quartos.HasValue)
                {
                    ModelState.AddModelError("Quartos", "N�mero de quartos � obrigat�rio para este tipo de im�vel");
                }
                if (!viewModel.Banheiros.HasValue)
                {
                    ModelState.AddModelError("Banheiros", "N�mero de banheiros � obrigat�rio para este tipo de im�vel");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Garantir que existe um locador v�lido
                    var locadorId = EnsureValidLocador(viewModel.LocadorId);
                    if (locadorId == 0)
                    {
                        ModelState.AddModelError("", "Erro: Nenhum locador encontrado. � necess�rio cadastrar um locador primeiro.");
                        return View(viewModel);
                    }

                    viewModel.LocadorId = locadorId;
                    var imovel = mapper.Map<Imovel>(viewModel);
                    imovelService.Create(imovel);
                    TempData["SuccessMessage"] = "Im�vel criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o im�vel: " + ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: Imovel/Edit/5
        public IActionResult Edit(int id)
        {
            var imovel = imovelService.Get(id);
            if (imovel == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<ImovelViewModel>(imovel);
            return View(viewModel);
        }

        // POST: Imovel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ImovelViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Valida��o condicional para tipo comercial: quartos/banheiros n�o obrigat�rios
            if (!viewModel.IsComercial)
            {
                if (!viewModel.Quartos.HasValue)
                {
                    ModelState.AddModelError("Quartos", "N�mero de quartos � obrigat�rio para este tipo de im�vel");
                }
                if (!viewModel.Banheiros.HasValue)
                {
                    ModelState.AddModelError("Banheiros", "N�mero de banheiros � obrigat�rio para este tipo de im�vel");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Garantir que existe um locador v�lido
                    var locadorId = EnsureValidLocador(viewModel.LocadorId);
                    if (locadorId == 0)
                    {
                        ModelState.AddModelError("", "Erro: Nenhum locador encontrado. � necess�rio cadastrar um locador primeiro.");
                        return View(viewModel);
                    }

                    viewModel.LocadorId = locadorId;
                    var imovel = mapper.Map<Imovel>(viewModel);
                    imovelService.Edit(imovel);
                    TempData["SuccessMessage"] = "Im�vel atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o im�vel: " + ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: Imovel/Delete/5
        public IActionResult Delete(int id)
        {
            var imovel = imovelService.Get(id);
            if (imovel == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<ImovelViewModel>(imovel);
            return View(viewModel);
        }

        // POST: Imovel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                imovelService.Delete(id);
                TempData["SuccessMessage"] = "Im�vel exclu�do com sucesso!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir im�vel: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        private int EnsureValidLocador(int? locadorId)
        {
            // Se foi fornecido um ID v�lido, verificar se existe
            if (locadorId.HasValue && locadorId.Value > 0)
            {
                var existingLocador = locadorService.Get(locadorId.Value);
                if (existingLocador != null)
                {
                    return locadorId.Value;
                }
            }

            // Tentar encontrar o primeiro locador dispon�vel
            var locadores = locadorService.GetAll(1, 1);
            var firstLocador = locadores.FirstOrDefault();
            if (firstLocador != null)
            {
                return firstLocador.Id;
            }

            // Se n�o h� locadores, criar um locador padr�o
            var defaultLocador = new Locador
            {
                Nome = "Locador Padr�o",
                Email = "locador@aluguelink.com",
                Telefone = "11999999999",
                Cpf = "00000000000"
            };

            try
            {
                return locadorService.Create(defaultLocador);
            }
            catch (Exception)
            {
                return 0; // Falha ao criar locador padr�o
            }
        }

        private bool PopulateLocadoresDropDownList(object? selectedLocador = null) => true; // m�todo legacy mantido por compatibilidade
    }
}