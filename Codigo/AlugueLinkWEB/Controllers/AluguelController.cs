using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlugueLinkWEB.Controllers
{
    [Authorize]
    public class AluguelController : Controller
    {
        private readonly IAluguelService aluguelService;
        private readonly ILocatarioService locatarioService;
        private readonly IImovelService imovelService;
        private readonly IMapper mapper;

        public AluguelController(IAluguelService aluguelService, ILocatarioService locatarioService, 
            IImovelService imovelService, IMapper mapper)
        {
            this.aluguelService = aluguelService;
            this.locatarioService = locatarioService;
            this.imovelService = imovelService;
            this.mapper = mapper;
        }

        // GET: Aluguel
        public IActionResult Index(int page = 1, int pageSize = 10, string? filtro = "todos")
        {
            try
            {
                // Atualizar status dos aluguéis antes de listar
                aluguelService.AtualizarStatusAlugueis();
            }
            catch (Exception)
            {
                // Continua mesmo se houver erro na atualização
            }

            var aluguels = aluguelService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<AluguelViewModel>>(aluguels).ToList();

            // Aplicar filtro - agora usando códigos diretos do banco
            filtro = (filtro ?? "todos").ToLowerInvariant();
            IEnumerable<AluguelViewModel> filtrados = viewModels;
            switch (filtro)
            {
                case "ativos":
                    filtrados = viewModels.Where(vm => vm.Status == "A");
                    break;
                case "finalizados":
                    filtrados = viewModels.Where(vm => vm.Status == "F");
                    break;
                case "pendentes":
                    filtrados = viewModels.Where(vm => vm.Status == "P");
                    break;
                default:
                    filtrados = viewModels;
                    break;
            }

            ViewBag.Filtro = filtro;
            ViewBag.TotalItems = aluguelService.GetCount();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(filtrados);
        }

        // GET: Aluguel/Details/5
        public IActionResult Details(int id)
        {
            var aluguel = aluguelService.Get(id);
            if (aluguel == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<AluguelViewModel>(aluguel);
            return View(viewModel);
        }

        // GET: Aluguel/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Aluguel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AluguelViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validações de negócio
                    if (!ValidateBusinessRules(viewModel))
                    {
                        PopulateDropDownLists(viewModel);
                        return View(viewModel);
                    }

                    var aluguel = mapper.Map<Aluguel>(viewModel);
                    aluguelService.Create(aluguel);
                    TempData["SuccessMessage"] = "Aluguel criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o aluguel: " + ex.Message);
                }
            }

            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // GET: Aluguel/Edit/5
        public IActionResult Edit(int id)
        {
            var aluguel = aluguelService.Get(id);
            if (aluguel == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<AluguelViewModel>(aluguel);
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Aluguel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AluguelViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validações de negócio
                    if (!ValidateBusinessRules(viewModel))
                    {
                        PopulateDropDownLists(viewModel);
                        return View(viewModel);
                    }

                    var aluguel = mapper.Map<Aluguel>(viewModel);
                    aluguelService.Edit(aluguel);
                    TempData["SuccessMessage"] = "Aluguel atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o aluguel: " + ex.Message);
                }
            }

            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // GET: Aluguel/Delete/5
        public IActionResult Delete(int id)
        {
            var aluguel = aluguelService.Get(id);
            if (aluguel == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<AluguelViewModel>(aluguel);
            return View(viewModel);
        }

        // POST: Aluguel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                aluguelService.Delete(id);
                TempData["SuccessMessage"] = "Aluguel excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir aluguel: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropDownLists(AluguelViewModel? viewModel = null)
        {
            // Obter lista de imóveis indisponíveis
            var imoveisIndisponiveis = aluguelService.GetImoveisIndisponiveis().ToList();

            // Carregar todos os locatários (agora podem ter múltiplos aluguéis)
            var todosLocatarios = locatarioService.GetAll(1, 1000);

            ViewBag.IdLocatario = new SelectList(todosLocatarios.Select(l => new { 
                Value = l.Id, 
                Text = $"{l.Nome} - {l.Email}"
            }), "Value", "Text", viewModel?.IdLocatario);

            // Carregar imóveis disponíveis
            var todosImoveis = imovelService.GetAll(1, 1000);
            var imoveisDisponiveis = todosImoveis.Where(i => 
                !imoveisIndisponiveis.Contains(i.Id) || 
                (viewModel?.IdImovel.HasValue == true && i.Id == viewModel.IdImovel.Value)
            );

            ViewBag.IdImovel = new SelectList(imoveisDisponiveis.Select(i => new { 
                Value = i.Id, 
                Text = $"{i.Logradouro}, {i.Numero} - {i.Bairro} (R$ {i.Valor:N2})" + (imoveisIndisponiveis.Contains(i.Id) ? " (Alugado)" : "")
            }), "Value", "Text", viewModel?.IdImovel);

            // Status - usar códigos do banco mas exibir nomes amigáveis
            ViewBag.Status = new SelectList(new[]
            {
                new { Value = "A", Text = "Ativo" },
                new { Value = "F", Text = "Finalizado" },
                new { Value = "P", Text = "Pendente" }
            }, "Value", "Text", viewModel?.Status);
        }

        private bool ValidateBusinessRules(AluguelViewModel viewModel)
        {
            bool isValid = true;

            // Verificar se o locatário existe
            if (viewModel.IdLocatario.HasValue)
            {
                var locatario = locatarioService.Get(viewModel.IdLocatario.Value);
                if (locatario == null)
                {
                    ModelState.AddModelError("IdLocatario", "Locatário selecionado não existe.");
                    isValid = false;
                }
            }

            // Verificar se o imóvel existe
            if (viewModel.IdImovel.HasValue)
            {
                var imovel = imovelService.Get(viewModel.IdImovel.Value);
                if (imovel == null)
                {
                    ModelState.AddModelError("IdImovel", "Imóvel selecionado não existe.");
                    isValid = false;
                }
                else
                {
                    // Verificar se o imóvel está disponível no período
                    if (!aluguelService.IsImovelAvailable(
                        viewModel.IdImovel.Value,
                        viewModel.DataInicio,
                        viewModel.DataFim,
                        viewModel.Id > 0 ? viewModel.Id : null))
                    {
                        ModelState.AddModelError("IdImovel", "Este imóvel já está alugado no período selecionado.");
                        isValid = false;
                    }
                }
            }

            // Validar datas
            if (viewModel.DataInicio.HasValue && viewModel.DataFim.HasValue)
            {
                if (viewModel.DataInicio >= viewModel.DataFim)
                {
                    ModelState.AddModelError("DataFim", "A data de fim deve ser posterior à data de início.");
                    isValid = false;
                }
            }

            if (viewModel.DataAssinatura.HasValue && viewModel.DataInicio.HasValue)
            {
                if (viewModel.DataAssinatura > viewModel.DataInicio)
                {
                    ModelState.AddModelError("DataAssinatura", "A data de assinatura não pode ser posterior à data de início.");
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}