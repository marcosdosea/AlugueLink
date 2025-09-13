using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers
{
    [Authorize]
    public class LocatarioController : Controller
    {
        private readonly ILocatarioService locatarioService;
        private readonly IAluguelService aluguelService;
        private readonly IMapper mapper;

        public LocatarioController(ILocatarioService locatarioService, IAluguelService aluguelService, IMapper mapper)
        {
            this.locatarioService = locatarioService;
            this.aluguelService = aluguelService;
            this.mapper = mapper;
        }

        // GET: Locatario
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var locatarios = locatarioService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<LocatarioViewModel>>(locatarios);

            var locatariosOcupados = aluguelService.GetLocatariosOcupados().ToList();
            
            foreach (var viewModel in viewModels)
            {
                viewModel.IsOcupado = locatariosOcupados.Contains(viewModel.Id);
                
                if (viewModel.IsOcupado)
                {
                    var aluguelAtivo = aluguelService.GetAluguelAtivoByLocatario(viewModel.Id);
                    if (aluguelAtivo != null)
                    {
                        viewModel.ImovelAtual = $"{aluguelAtivo.IdimovelNavigation?.Logradouro}, {aluguelAtivo.IdimovelNavigation?.Numero}";
                        viewModel.DataInicioAluguel = aluguelAtivo.DataInicio;
                        viewModel.DataFimAluguel = aluguelAtivo.DataFim;
                    }
                }
            }

            ViewBag.TotalItems = locatarioService.GetCount();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(viewModels);
        }

        // GET: Locatario/Details/5
        public IActionResult Details(int id)
        {
            var locatario = locatarioService.Get(id);
            if (locatario == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<LocatarioViewModel>(locatario);
            
            viewModel.IsOcupado = !aluguelService.IsLocatarioAvailable(id);
            if (viewModel.IsOcupado)
            {
                var aluguelAtivo = aluguelService.GetAluguelAtivoByLocatario(id);
                if (aluguelAtivo != null)
                {
                    viewModel.ImovelAtual = $"{aluguelAtivo.IdimovelNavigation?.Logradouro}, {aluguelAtivo.IdimovelNavigation?.Numero}";
                    viewModel.DataInicioAluguel = aluguelAtivo.DataInicio;
                    viewModel.DataFimAluguel = aluguelAtivo.DataFim;
                }
            }
            
            return View(viewModel);
        }

        // GET: Locatario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locatario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LocatarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.Telefone2) && viewModel.Telefone1 == viewModel.Telefone2)
                {
                    ModelState.AddModelError("Telefone2", "O telefone secundário deve ser diferente do principal.");
                    return View(viewModel);
                }
                try
                {
                    var locatario = mapper.Map<Locatario>(viewModel);
                    locatarioService.Create(locatario);
                    TempData["SuccessMessage"] = "Locatário criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o locatário: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Locatario/Edit/5
        public IActionResult Edit(int id)
        {
            var locatario = locatarioService.Get(id);
            if (locatario == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<LocatarioViewModel>(locatario);
            return View(viewModel);
        }

        // POST: Locatario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LocatarioViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.Telefone2) && viewModel.Telefone1 == viewModel.Telefone2)
                {
                    ModelState.AddModelError("Telefone2", "O telefone secundário deve ser diferente do principal.");
                    return View(viewModel);
                }
                try
                {
                    var locatario = mapper.Map<Locatario>(viewModel);
                    locatarioService.Edit(locatario);
                    TempData["SuccessMessage"] = "Locatário atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o locatário: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Locatario/Delete/5
        public IActionResult Delete(int id)
        {
            var locatario = locatarioService.Get(id);
            if (locatario == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<LocatarioViewModel>(locatario);
            return View(viewModel);
        }

        // POST: Locatario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                locatarioService.Delete(id);
                TempData["SuccessMessage"] = "Inquilino excluído com sucesso!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir inquilino: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}