using Microsoft.AspNetCore.Mvc;
using Core.Service;
using AlugueLinkWEB.Models;
using AlugueLinkWEB.Mappers;

namespace AlugueLinkWEB.Controllers
{
    public class LocatarioController : Controller
    {
        private readonly ILocatarioService _locatarioService;

        public LocatarioController(ILocatarioService locatarioService)
        {
            _locatarioService = locatarioService;
        }

        // GET: Locatario
        public async Task<IActionResult> Index()
        {
            var locatarios = await _locatarioService.GetAllAsync();
            var viewModels = LocatarioMapper.ToViewModelList(locatarios);
            return View(viewModels);
        }

        // GET: Locatario/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var locatario = await _locatarioService.GetByIdAsync(id);
            if (locatario == null)
            {
                return NotFound();
            }

            var viewModel = LocatarioMapper.ToViewModel(locatario);
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
        public async Task<IActionResult> Create(LocatarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dto = LocatarioMapper.ToDTO(viewModel);
                await _locatarioService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Locatário criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Locatario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var locatario = await _locatarioService.GetByIdAsync(id);
            if (locatario == null)
            {
                return NotFound();
            }

            var viewModel = LocatarioMapper.ToViewModel(locatario);
            return View(viewModel);
        }

        // POST: Locatario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LocatarioViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dto = LocatarioMapper.ToDTO(viewModel);
                var resultado = await _locatarioService.UpdateAsync(id, dto);
                
                if (resultado == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Locatário atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Locatario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var locatario = await _locatarioService.GetByIdAsync(id);
            if (locatario == null)
            {
                return NotFound();
            }

            var viewModel = LocatarioMapper.ToViewModel(locatario);
            return View(viewModel);
        }

        // POST: Locatario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resultado = await _locatarioService.DeleteAsync(id);
            if (resultado)
            {
                TempData["SuccessMessage"] = "Locatário excluído com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Erro ao excluir locatário.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}