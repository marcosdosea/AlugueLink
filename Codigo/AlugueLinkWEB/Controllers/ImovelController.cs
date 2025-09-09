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
                    // Garantir que existe um locador válido
                    var locadorId = EnsureValidLocador(viewModel.LocadorId);
                    if (locadorId == 0)
                    {
                        ModelState.AddModelError("", "Erro: Nenhum locador encontrado. É necessário cadastrar um locador primeiro.");
                        return View(viewModel);
                    }

                    viewModel.LocadorId = locadorId;
                    var imovel = mapper.Map<Imovel>(viewModel);
                    imovelService.Create(imovel);
                    TempData["SuccessMessage"] = "Imóvel criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar o imóvel: " + ex.Message);
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
                    // Garantir que existe um locador válido
                    var locadorId = EnsureValidLocador(viewModel.LocadorId);
                    if (locadorId == 0)
                    {
                        ModelState.AddModelError("", "Erro: Nenhum locador encontrado. É necessário cadastrar um locador primeiro.");
                        return View(viewModel);
                    }

                    viewModel.LocadorId = locadorId;
                    var imovel = mapper.Map<Imovel>(viewModel);
                    imovelService.Edit(imovel);
                    TempData["SuccessMessage"] = "Imóvel atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o imóvel: " + ex.Message);
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
            TempData["SuccessMessage"] = "Imóvel excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private int EnsureValidLocador(int? locadorId)
        {
            // Se foi fornecido um ID válido, verificar se existe
            if (locadorId.HasValue && locadorId.Value > 0)
            {
                var existingLocador = locadorService.Get(locadorId.Value);
                if (existingLocador != null)
                {
                    return locadorId.Value;
                }
            }

            // Tentar encontrar o primeiro locador disponível
            var locadores = locadorService.GetAll(1, 1);
            var firstLocador = locadores.FirstOrDefault();
            if (firstLocador != null)
            {
                return firstLocador.Id;
            }

            // Se não há locadores, criar um locador padrão
            var defaultLocador = new Locador
            {
                Nome = "Locador Padrão",
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
                return 0; // Falha ao criar locador padrão
            }
        }

        private bool PopulateLocadoresDropDownList(object? selectedLocador = null) => true; // método legacy mantido por compatibilidade
    }
}