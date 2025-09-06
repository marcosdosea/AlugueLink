using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers
{
    public class LocatarioController : Controller
    {
        private readonly ILocatarioService locatarioService;
        private readonly IMapper mapper;

        public LocatarioController(ILocatarioService locatarioService, IMapper mapper)
        {
            this.locatarioService = locatarioService;
            this.mapper = mapper;
        }

        // GET: Locatario
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var locatarios = locatarioService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<LocatarioViewModel>>(locatarios);

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
            locatarioService.Delete(id);
            TempData["SuccessMessage"] = "Locatário excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}