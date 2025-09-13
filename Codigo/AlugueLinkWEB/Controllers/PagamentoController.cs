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
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService pagamentoService;
        private readonly IAluguelService aluguelService;
        private readonly ILocadorService locadorService;
        private readonly IMapper mapper;

        public PagamentoController(IPagamentoService pagamentoService, IAluguelService aluguelService, 
            ILocadorService locadorService, IMapper mapper)
        {
            this.pagamentoService = pagamentoService;
            this.aluguelService = aluguelService;
            this.locadorService = locadorService;
            this.mapper = mapper;
        }

        // GET: Pagamento
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                var pagamentos = pagamentoService.GetByLocador(locador.Id, page, pageSize);
                var viewModels = new List<PagamentoViewModel>();

                // Processar cada pagamento de forma segura
                foreach (var pagamento in pagamentos)
                {
                    try
                    {
                        // Mapeamento manual para evitar problemas do AutoMapper
                        var viewModel = new PagamentoViewModel
                        {
                            Id = pagamento.Id,
                            Valor = pagamento.Valor,
                            ValorStr = pagamento.Valor.ToString("F2"),
                            TipoPagamento = pagamento.TipoPagamento,
                            AluguelId = pagamento.Idaluguel,
                            DataHoraPagamento = pagamento.DataPagamento
                        };

                        if (pagamento.DataPagamento != default(DateTime))
                        {
                            viewModel.DataPagamento = DateOnly.FromDateTime(pagamento.DataPagamento);
                            viewModel.HoraPagamento = TimeOnly.FromDateTime(pagamento.DataPagamento);
                        }
                                                
                        var aluguel = pagamento.IdaluguelNavigation;
                        if (aluguel != null)
                        {
                            var imovel = aluguel.IdimovelNavigation;
                            var locatario = aluguel.IdlocatarioNavigation;

                            if (imovel != null)
                            {
                                viewModel.ImovelEndereco = $"{imovel.Logradouro}, {imovel.Numero} - {imovel.Bairro}, {imovel.Cidade}";
                                viewModel.ValorAluguel = imovel.Valor;
                            }

                            if (locatario != null)
                            {
                                viewModel.LocatarioNome = locatario.Nome;
                            }
                            else
                            {
                                viewModel.LocatarioNome = "Inquilino não encontrado";
                            }

                            viewModel.DataInicioAluguel = aluguel.DataInicio;
                            viewModel.DataFimAluguel = aluguel.DataFim;
                            viewModel.StatusAluguel = aluguel.Status;
                        }

                        viewModels.Add(viewModel);
                    }
                    catch
                    {

                    }
                }

                ViewBag.TotalItems = pagamentoService.GetCountByLocador(locador.Id);
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.LocadorNome = locador.Nome;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro interno: " + ex.Message;
                return View(new List<PagamentoViewModel>());
            }
        }

        // GET: Pagamento/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                var pagamento = pagamentoService.Get(id);
                if (pagamento == null)
                    return NotFound();

                if (pagamento.IdaluguelNavigation?.IdimovelNavigation?.IdLocador != locador.Id)
                {
                    TempData["ErrorMessage"] = "Acesso negado: Este pagamento não pertence a você.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new PagamentoViewModel
                {
                    Id = pagamento.Id,
                    Valor = pagamento.Valor,
                    ValorStr = pagamento.Valor.ToString("F2"),
                    TipoPagamento = pagamento.TipoPagamento,
                    AluguelId = pagamento.Idaluguel,
                    DataHoraPagamento = pagamento.DataPagamento
                };

                if (pagamento.DataPagamento != default(DateTime))
                {
                    viewModel.DataPagamento = DateOnly.FromDateTime(pagamento.DataPagamento);
                    viewModel.HoraPagamento = TimeOnly.FromDateTime(pagamento.DataPagamento);
                }
                
                var aluguel = pagamento.IdaluguelNavigation;
                if (aluguel != null)
                {
                    var imovel = aluguel.IdimovelNavigation;
                    var locatario = aluguel.IdlocatarioNavigation;

                    if (imovel != null)
                    {
                        viewModel.ImovelEndereco = $"{imovel.Logradouro}, {imovel.Numero} - {imovel.Bairro}, {imovel.Cidade}";
                        viewModel.ValorAluguel = imovel.Valor;
                    }

                    if (locatario != null)
                    {
                        viewModel.LocatarioNome = locatario.Nome;
                    }

                    viewModel.DataInicioAluguel = aluguel.DataInicio;
                    viewModel.DataFimAluguel = aluguel.DataFim;
                    viewModel.StatusAluguel = aluguel.Status;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar detalhes do pagamento: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Pagamento/Create
        public IActionResult Create()
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                if (!PopulateAlugueisList(locador.Id))
                {
                    TempData["InfoMessage"] = "Para registrar pagamentos, é necessário ter aluguéis ativos. Cadastre primeiro um imóvel e um contrato de aluguel.";
                    return RedirectToAction(nameof(Index));
                }

                PopulateTiposPagamentoList();
                
                var viewModel = new PagamentoViewModel
                {
                    DataPagamento = DateOnly.FromDateTime(DateTime.Now),
                    HoraPagamento = TimeOnly.FromDateTime(DateTime.Now)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar página de criação: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Pagamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PagamentoViewModel viewModel)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                if (!viewModel.AluguelId.HasValue || viewModel.AluguelId.Value <= 0)
                {
                    ModelState.AddModelError("AluguelId", "Selecione um aluguel válido.");
                }

                if (string.IsNullOrEmpty(viewModel.TipoPagamento))
                {
                    ModelState.AddModelError("TipoPagamento", "Selecione um tipo de pagamento.");
                }

                if (!viewModel.DataPagamento.HasValue)
                {
                    ModelState.AddModelError("DataPagamento", "Informe a data do pagamento.");
                }

                if (!viewModel.HoraPagamento.HasValue)
                {
                    ModelState.AddModelError("HoraPagamento", "Informe a hora do pagamento.");
                }

                if (viewModel.AluguelId.HasValue && viewModel.AluguelId.Value > 0)
                {
                    var aluguel = aluguelService.Get(viewModel.AluguelId.Value);
                    if (aluguel?.IdimovelNavigation?.IdLocador != locador.Id)
                    {
                        ModelState.AddModelError("AluguelId", "Aluguel não encontrado ou não pertence a você.");
                    }
                    else if (aluguel.IdimovelNavigation != null)
                    {
                        viewModel.Valor = aluguel.IdimovelNavigation.Valor;
                        viewModel.ValorStr = aluguel.IdimovelNavigation.Valor.ToString("F2");
                        viewModel.ValorAluguel = aluguel.IdimovelNavigation.Valor;
                    }
                }

                if (!viewModel.Valor.HasValue || viewModel.Valor.Value <= 0)
                {
                    ModelState.AddModelError("Valor", "Valor do pagamento deve ser maior que zero.");
                }

                if (!ModelState.IsValid)
                {
                    PopulateAlugueisList(locador.Id, viewModel.AluguelId);
                    PopulateTiposPagamentoList(viewModel.TipoPagamento);
                    return View(viewModel);
                }

                var pagamento = new Pagamento
                {
                    Valor = viewModel.Valor.Value,
                    DataPagamento = viewModel.DataHoraPagamento ?? DateTime.Now,
                    TipoPagamento = viewModel.TipoPagamento,
                    Idaluguel = viewModel.AluguelId.Value
                };
                
                var aluguelExiste = aluguelService.Get(pagamento.Idaluguel);
                if (aluguelExiste == null)
                {
                    ModelState.AddModelError("AluguelId", "Aluguel selecionado não existe.");
                    PopulateAlugueisList(locador.Id, viewModel.AluguelId);
                    PopulateTiposPagamentoList(viewModel.TipoPagamento);
                    return View(viewModel);
                }
                
                var novoId = pagamentoService.Create(pagamento);
                
                TempData["SuccessMessage"] = "Pagamento registrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar o pagamento: " + ex.Message);
                
                try
                {
                    var userEmail = User.Identity?.Name;
                    var locador = EnsureValidLocador(userEmail);
                    if (locador != null)
                    {
                        PopulateAlugueisList(locador.Id, viewModel.AluguelId);
                        PopulateTiposPagamentoList(viewModel.TipoPagamento);
                    }
                }
                catch
                {
                
                }
                
                return View(viewModel);
            }
        }

        // GET: Pagamento/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                var pagamento = pagamentoService.Get(id);
                if (pagamento == null)
                    return NotFound();

                if (pagamento.IdaluguelNavigation?.IdimovelNavigation?.IdLocador != locador.Id)
                {
                    TempData["ErrorMessage"] = "Acesso negado: Este pagamento não pertence a você.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new PagamentoViewModel
                {
                    Id = pagamento.Id,
                    Valor = pagamento.Valor,
                    ValorStr = pagamento.Valor.ToString("F2"),
                    TipoPagamento = pagamento.TipoPagamento,
                    AluguelId = pagamento.Idaluguel,
                    DataHoraPagamento = pagamento.DataPagamento
                };

                if (pagamento.DataPagamento != default(DateTime))
                {
                    viewModel.DataPagamento = DateOnly.FromDateTime(pagamento.DataPagamento);
                    viewModel.HoraPagamento = TimeOnly.FromDateTime(pagamento.DataPagamento);
                }
                
                if (pagamento.IdaluguelNavigation?.IdimovelNavigation != null)
                {
                    viewModel.ValorAluguel = pagamento.IdaluguelNavigation.IdimovelNavigation.Valor;
                }

                PopulateAlugueisList(locador.Id, viewModel.AluguelId);
                PopulateTiposPagamentoList(viewModel.TipoPagamento);
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar página de edição: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Pagamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PagamentoViewModel viewModel)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                if (id != viewModel.Id) 
                    return NotFound();

                var existingPagamento = pagamentoService.Get(id);
                if (existingPagamento?.IdaluguelNavigation?.IdimovelNavigation?.IdLocador != locador.Id)
                {
                    TempData["ErrorMessage"] = "Acesso negado: Este pagamento não pertence a você.";
                    return RedirectToAction(nameof(Index));
                }

                if (viewModel.AluguelId.HasValue)
                {
                    var aluguel = aluguelService.Get(viewModel.AluguelId.Value);
                    if (aluguel?.IdimovelNavigation?.IdLocador != locador.Id)
                    {
                        ModelState.AddModelError("AluguelId", "Aluguel não encontrado ou não pertence a você.");
                    }
                    else if (aluguel.IdimovelNavigation != null)
                    {
                        viewModel.Valor = aluguel.IdimovelNavigation.Valor;
                        viewModel.ValorStr = aluguel.IdimovelNavigation.Valor.ToString("F2");
                        viewModel.ValorAluguel = aluguel.IdimovelNavigation.Valor;
                    }
                }

                if (ModelState.IsValid)
                {
                    var pagamento = new Pagamento
                    {
                        Id = viewModel.Id,
                        Valor = viewModel.Valor ?? 0,
                        DataPagamento = viewModel.DataHoraPagamento ?? DateTime.Now,
                        TipoPagamento = viewModel.TipoPagamento,
                        Idaluguel = viewModel.AluguelId ?? 0
                    };
                    
                    pagamentoService.Edit(pagamento);
                    TempData["SuccessMessage"] = "Pagamento atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                PopulateAlugueisList(locador.Id, viewModel.AluguelId);
                PopulateTiposPagamentoList(viewModel.TipoPagamento);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao atualizar o pagamento: " + ex.Message);
                
                try
                {
                    var userEmail = User.Identity?.Name;
                    var locador = EnsureValidLocador(userEmail);
                    if (locador != null)
                    {
                        PopulateAlugueisList(locador.Id, viewModel.AluguelId);
                        PopulateTiposPagamentoList(viewModel.TipoPagamento);
                    }
                }
                catch { }
                
                return View(viewModel);
            }
        }

        // GET: Pagamento/Delete/5
        public IActionResult Delete(int id)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                var pagamento = pagamentoService.Get(id);
                if (pagamento == null)
                    return NotFound();

                if (pagamento.IdaluguelNavigation?.IdimovelNavigation?.IdLocador != locador.Id)
                {
                    TempData["ErrorMessage"] = "Acesso negado: Este pagamento não pertence a você.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new PagamentoViewModel
                {
                    Id = pagamento.Id,
                    Valor = pagamento.Valor,
                    ValorStr = pagamento.Valor.ToString("F2"),
                    TipoPagamento = pagamento.TipoPagamento,
                    AluguelId = pagamento.Idaluguel,
                    DataHoraPagamento = pagamento.DataPagamento
                };

                if (pagamento.DataPagamento != default(DateTime))
                {
                    viewModel.DataPagamento = DateOnly.FromDateTime(pagamento.DataPagamento);
                    viewModel.HoraPagamento = TimeOnly.FromDateTime(pagamento.DataPagamento);
                }
                
                var aluguel = pagamento.IdaluguelNavigation;
                if (aluguel != null)
                {
                    var imovel = aluguel.IdimovelNavigation;
                    var locatario = aluguel.IdlocatarioNavigation;

                    if (imovel != null)
                    {
                        viewModel.ImovelEndereco = $"{imovel.Logradouro}, {imovel.Numero} - {imovel.Bairro}, {imovel.Cidade}";
                        viewModel.ValorAluguel = imovel.Valor;
                    }

                    if (locatario != null)
                    {
                        viewModel.LocatarioNome = locatario.Nome;
                    }

                    viewModel.DataInicioAluguel = aluguel.DataInicio;
                    viewModel.DataFimAluguel = aluguel.DataFim;
                    viewModel.StatusAluguel = aluguel.Status;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar página de exclusão: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Pagamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Erro: Usuário não identificado.";
                    return RedirectToAction("Index", "Home");
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    TempData["ErrorMessage"] = "Erro interno: Não foi possível criar ou localizar dados do locador.";
                    return RedirectToAction("Index", "Home");
                }

                var pagamento = pagamentoService.Get(id);
                if (pagamento?.IdaluguelNavigation?.IdimovelNavigation?.IdLocador != locador.Id)
                {
                    TempData["ErrorMessage"] = "Acesso negado: Este pagamento não pertence a você.";
                    return RedirectToAction(nameof(Index));
                }

                pagamentoService.Delete(id);
                TempData["SuccessMessage"] = "Pagamento excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir pagamento: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        //Obter valor do aluguel selecionado
        [HttpGet]
        public IActionResult GetValorAluguel(int aluguelId)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "Usuário não identificado" });
                }

                var locador = EnsureValidLocador(userEmail);
                if (locador == null)
                {
                    return Json(new { success = false, message = "Locador não encontrado" });
                }

                var aluguel = aluguelService.Get(aluguelId);
                if (aluguel?.IdimovelNavigation?.IdLocador != locador.Id)
                {
                    return Json(new { success = false, message = "Aluguel não encontrado ou não autorizado" });
                }

                var valor = aluguel.IdimovelNavigation.Valor;
                return Json(new { 
                    success = true, 
                    valor = valor.ToString("F2"),
                    valorFormatado = valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"))
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro interno: " + ex.Message });
            }
        }


        private Locador? EnsureValidLocador(string userEmail)
        {
            try
            {
                var locador = locadorService.GetByEmail(userEmail);
                if (locador != null) 
                    return locador;

                var locadores = locadorService.GetAll(1, 1);
                var firstLocador = locadores.FirstOrDefault();
                if (firstLocador != null) 
                    return firstLocador;

                var nomeUsuario = User.Identity?.Name ?? userEmail;
                var nomeCompleto = User.Claims?.FirstOrDefault(c => c.Type == "NomeCompleto")?.Value;
                
                var novoLocador = new Locador
                {
                    Nome = nomeCompleto ?? nomeUsuario.Split('@')[0], 
                    Email = userEmail,
                    Telefone = "11999999999", 
                    Cpf = "00000000000" // 
                };

                var locadorId = locadorService.Create(novoLocador);
                return locadorService.Get(locadorId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool PopulateAlugueisList(int locadorId, object? selectedAluguel = null)
        {
            try
            {
                var alugueis = aluguelService.GetByLocador(locadorId)
                    .Where(a => a.Status == "A") 
                    .ToList();

                if (!alugueis.Any())
                    return false;

                var aluguelItems = alugueis.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.IdimovelNavigation?.Logradouro} - {a.IdlocatarioNavigation?.Nome ?? "Inquilino não definido"} (R$ {a.IdimovelNavigation?.Valor:F2})",
                    Selected = selectedAluguel?.ToString() == a.Id.ToString()
                });

                ViewBag.AluguelId = new SelectList(aluguelItems, "Value", "Text", selectedAluguel);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void PopulateTiposPagamentoList(object? selectedTipo = null)
        {
            var tipos = new List<SelectListItem>
            {
                new SelectListItem { Value = "CD", Text = "Cartão de Débito" },
                new SelectListItem { Value = "CC", Text = "Cartão de Crédito" },
                new SelectListItem { Value = "P", Text = "PIX" },
                new SelectListItem { Value = "B", Text = "Boleto" }
            };

            ViewBag.TipoPagamento = new SelectList(tipos, "Value", "Text", selectedTipo);
        }
    }
}