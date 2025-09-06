using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers
{
    public class LocadorController : Controller
    {
        private readonly ILocadorService locadorService;
        private readonly IMapper mapper;

        public LocadorController(ILocadorService locadorService, IMapper mapper)
        {
            this.locadorService = locadorService;
            this.mapper = mapper;
        }

        // GET: Locador
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var locadores = locadorService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<LocadorViewModel>>(locadores);

            ViewBag.TotalItems = locadorService.GetCount();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(viewModels);
        }

        // GET: Locador/Details/5
        public IActionResult Details(int id)
        {
            var locador = locadorService.Get(id);
            if (locador == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<LocadorViewModel>(locador);
            return View(viewModel);
        }

        // GET: Locador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LocadorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var locador = mapper.Map<Locador>(viewModel);
                    locadorService.Create(locador);
                    TempData["SuccessMessage"] = "Locador criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o locador: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Locador/Edit/5
        public IActionResult Edit(int id)
        {
            var locador = locadorService.Get(id);
            if (locador == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<LocadorViewModel>(locador);
            return View(viewModel);
        }

        // POST: Locador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LocadorViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var locador = mapper.Map<Locador>(viewModel);
                    locadorService.Edit(locador);
                    TempData["SuccessMessage"] = "Locador atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o locador: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Locador/Delete/5
        public IActionResult Delete(int id)
        {
            var locador = locadorService.Get(id);
            if (locador == null)
            {
                return NotFound();
            }

            var viewModel = mapper.Map<LocadorViewModel>(locador);
            return View(viewModel);
        }

        // POST: Locador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            locadorService.Delete(id);
            TempData["SuccessMessage"] = "Locador excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}