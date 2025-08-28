using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AlugueLinkWEB.Models;
using AlugueLinkWEB.Mappers;
using Core.Service;
using Core;
using Microsoft.EntityFrameworkCore;

namespace AlugueLinkWEB.Controllers
{
    public class ImovelController : Controller
    {
        private readonly IImovelService _imovelService;
        private readonly AluguelinkContext _context;

        public ImovelController(IImovelService imovelService, AluguelinkContext context)
        {
            _imovelService = imovelService;
            _context = context;
        }

        // GET: Imovel
        public async Task<IActionResult> Index()
        {
            var imoveisDto = await _imovelService.GetAllAsync();
            var imoveisViewModel = ImovelMapper.ToViewModelList(imoveisDto);

            // Carrega os nomes dos locadores para exibição
            var locadores = await _context.Locadors.ToListAsync();
            foreach (var imovel in imoveisViewModel)
            {
                var locador = locadores.FirstOrDefault(l => l.Id == imovel.LocadorId);
                imovel.LocadorNome = locador?.Nome;
            }

            return View(imoveisViewModel);
        }

        // GET: Imovel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovelDto = await _imovelService.GetByIdAsync(id.Value);
            if (imovelDto == null)
            {
                return NotFound();
            }

            var imovelViewModel = ImovelMapper.ToViewModel(imovelDto);
            
            // Carrega o nome do locador para exibição
            var locador = await _context.Locadors.FindAsync(imovelViewModel.LocadorId);
            imovelViewModel.LocadorNome = locador?.Nome;

            return View(imovelViewModel);
        }

        // GET: Imovel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Imovel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImovelViewModel imovelViewModel)
        {
            // Garantir que existe pelo menos um locador
            var locadorId = await EnsureLocadorExists();
            if (locadorId == 0)
            {
                ModelState.AddModelError("", "Erro ao configurar locador padrão no sistema.");
                return View(imovelViewModel);
            }

            // Definir o LocadorId
            imovelViewModel.LocadorId = locadorId;

            if (ModelState.IsValid)
            {
                try
                {
                    var imovelDto = ImovelMapper.ToDTO(imovelViewModel);
                    await _imovelService.CreateAsync(imovelDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o imóvel: " + ex.Message);
                }
            }

            return View(imovelViewModel);
        }

        // GET: Imovel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovelDto = await _imovelService.GetByIdAsync(id.Value);
            if (imovelDto == null)
            {
                return NotFound();
            }

            var imovelViewModel = ImovelMapper.ToViewModel(imovelDto);
            await PopulateLocadoresDropDownList(imovelViewModel.LocadorId);
            return View(imovelViewModel);
        }

        // POST: Imovel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ImovelViewModel imovelViewModel)
        {
            if (id != imovelViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var imovelDto = ImovelMapper.ToDTO(imovelViewModel);
                    var result = await _imovelService.UpdateAsync(id, imovelDto);
                    if (result == null)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _imovelService.GetByIdAsync(id);
                    if (exists == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateLocadoresDropDownList(imovelViewModel.LocadorId);
            return View(imovelViewModel);
        }

        // GET: Imovel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovelDto = await _imovelService.GetByIdAsync(id.Value);
            if (imovelDto == null)
            {
                return NotFound();
            }

            var imovelViewModel = ImovelMapper.ToViewModel(imovelDto);
            
            // Carrega o nome do locador para exibição
            var locador = await _context.Locadors.FindAsync(imovelViewModel.LocadorId);
            imovelViewModel.LocadorNome = locador?.Nome;

            return View(imovelViewModel);
        }

        // POST: Imovel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _imovelService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateLocadoresDropDownList(object? selectedLocador = null)
        {
            var locadores = await _context.Locadors
                .OrderBy(l => l.Nome)
                .Select(l => new { l.Id, l.Nome })
                .ToListAsync();

            ViewBag.LocadorId = new SelectList(locadores, "Id", "Nome", selectedLocador);
        }

        private async Task<int> EnsureLocadorExists()
        {
            var locadorExistente = await _context.Locadors.FirstOrDefaultAsync();
            if (locadorExistente != null)
            {
                return locadorExistente.Id;
            }

            try
            {
                var locadorPadrao = new Locador
                {
                    Nome = "Locador Padrão",
                    Email = "padrao@aluguelink.com",
                    Cpf = "00000000000",
                    Telefone = "00000000000"
                };

                _context.Locadors.Add(locadorPadrao);
                await _context.SaveChangesAsync();

                return locadorPadrao.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}