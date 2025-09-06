using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers
{
    public class ImovelController : Controller
    {
        private readonly IImovelService imovelService;
        private readonly ILocadorService locadorService;
        private readonly IMapper mapper;

        public ImovelController(IImovelService imovelService, ILocadorService locadorService, IMapper mapper)
        {
            this.imovelService = imovelService;
            this.locadorService = locadorService;
            this.mapper = mapper;
        }

        // GET: Imovel
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var imoveis = imovelService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<ImovelViewModel>>(imoveis);

            ViewBag.TotalItems = imovelService.GetCount();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(viewModels);
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
            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.LocadorId == null || viewModel.LocadorId == 0)
                    {
                        viewModel.LocadorId = 1; // locador padr�o
                    }
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

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.LocadorId == null || viewModel.LocadorId == 0)
                    {
                        viewModel.LocadorId = 1;
                    }
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
            imovelService.Delete(id);
            TempData["SuccessMessage"] = "Im�vel exclu�do com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private bool PopulateLocadoresDropDownList(object? selectedLocador = null) => true; // m�todo legacy mantido por compatibilidade
    }
}