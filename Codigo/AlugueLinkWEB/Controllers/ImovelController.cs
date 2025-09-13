using AutoMapper;
using Core;
using Core.Service;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;

namespace AlugueLinkWEB.Controllers
{
    [Authorize]
    public class ImovelController : Controller
    {
        private readonly IImovelService imovelService;
        private readonly ILocadorService locadorService;
        private readonly IAluguelService aluguelService;
        private readonly IMapper mapper;

        private static readonly Regex DecimalPattern = new Regex(@"^\d+,\d{1,2}$", RegexOptions.Compiled);
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

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
            try { aluguelService.AtualizarStatusAlugueis(); } catch (Exception) { }
            var imoveis = imovelService.GetAll(page, pageSize);
            var viewModels = mapper.Map<IEnumerable<ImovelViewModel>>(imoveis).ToList();
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
            filtro = (filtro ?? "todos").ToLowerInvariant();
            IEnumerable<ImovelViewModel> filtrados = filtro switch
            {
                "alugados" => viewModels.Where(vm => vm.IsAlugado),
                "disponiveis" or "disponíveis" => viewModels.Where(vm => !vm.IsAlugado),
                _ => viewModels
            };
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
            if (imovel == null) return NotFound();
            var viewModel = mapper.Map<ImovelViewModel>(imovel);
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
        public IActionResult Create() => View();

        // POST: Imovel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ImovelViewModel viewModel)
        {
            ValidateDecimalRawFormat("ValorStr");
            ValidateDecimalRawFormat("AreaStr");
            ParseDecimalFields(viewModel);
            ValidateQuartosBanheiros(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
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
            if (imovel == null) return NotFound();
            var viewModel = mapper.Map<ImovelViewModel>(imovel);
            return View(viewModel);
        }

        // POST: Imovel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ImovelViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();
            ValidateDecimalRawFormat("ValorStr");
            ValidateDecimalRawFormat("AreaStr");
            ParseDecimalFields(viewModel);
            ValidateQuartosBanheiros(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
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
            if (imovel == null) return NotFound();
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
                TempData["SuccessMessage"] = "Imóvel excluído com sucesso!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir imóvel: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private void ValidateQuartosBanheiros(ImovelViewModel viewModel)
        {
            if (!viewModel.IsComercial)
            {
                if (!viewModel.Quartos.HasValue)
                    ModelState.AddModelError("Quartos", "Número de quartos é obrigatório para este tipo de imóvel");
                if (!viewModel.Banheiros.HasValue)
                    ModelState.AddModelError("Banheiros", "Número de banheiros é obrigatório para este tipo de imóvel");
            }
        }

        private int EnsureValidLocador(int? locadorId)
        {
            if (locadorId.HasValue && locadorId.Value > 0)
            {
                var existingLocador = locadorService.Get(locadorId.Value);
                if (existingLocador != null) return locadorId.Value;
            }
            var locadores = locadorService.GetAll(1, 1);
            var firstLocador = locadores.FirstOrDefault();
            if (firstLocador != null) return firstLocador.Id;
            var defaultLocador = new Locador
            {
                Nome = "Locador Padrão",
                Email = "locador@aluguelink.com",
                Telefone = "11999999999",
                Cpf = "00000000000"
            };
            try { return locadorService.Create(defaultLocador); } catch { return 0; }
        }

        private bool PopulateLocadoresDropDownList(object? selectedLocador = null) => true;

        private void ValidateDecimalRawFormat(string fieldName)
        {
            var raw = Request.Form[fieldName];
            if (string.IsNullOrWhiteSpace(raw)) return;
            raw = raw.ToString().Trim();
            if (!DecimalPattern.IsMatch(raw))
            {
                if (ModelState.TryGetValue(fieldName, out var entry)) entry.Errors.Clear();
                ModelState.AddModelError(fieldName, "Formato inválido. Use vírgula e 1 ou 2 casas decimais (ex: 123,4 ou 123,45).");
            }
        }

        private void ParseDecimalFields(ImovelViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.AreaStr) && DecimalPattern.IsMatch(vm.AreaStr))
            {
                if (decimal.TryParse(vm.AreaStr, NumberStyles.Number, PtBr, out var area)) vm.Area = area;
                else ModelState.AddModelError("AreaStr", "Não foi possível interpretar a área.");
            }
            if (!string.IsNullOrWhiteSpace(vm.ValorStr) && DecimalPattern.IsMatch(vm.ValorStr))
            {
                if (decimal.TryParse(vm.ValorStr, NumberStyles.Number, PtBr, out var valor)) vm.Valor = valor;
                else ModelState.AddModelError("ValorStr", "Não foi possível interpretar o valor.");
            }
        }
    }
}