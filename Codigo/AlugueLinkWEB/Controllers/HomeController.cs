using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AlugueLinkWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Core.Service;

namespace AlugueLinkWEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IImovelService _imovelService;
        private readonly ILocatarioService _locatarioService;
        private readonly IAluguelService _aluguelService;

        public HomeController(ILogger<HomeController> logger,
            IImovelService imovelService,
            ILocatarioService locatarioService,
            IAluguelService aluguelService)
        {
            _logger = logger;
            _imovelService = imovelService;
            _locatarioService = locatarioService;
            _aluguelService = aluguelService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    // Atualizar status dos aluguéis antes de calcular estatísticas
                    _aluguelService.AtualizarStatusAlugueis();
                }
                catch (Exception ex)
                {
                    // Log do erro mas não interrompe a aplicação
                    _logger.LogWarning(ex, "Erro ao atualizar status dos aluguéis");
                }

                // Calcular estatísticas para usuários logados
                var totalImoveis = _imovelService.GetCount();
                var totalLocatarios = _locatarioService.GetCount();
                var imoveisAlugados = _aluguelService.GetImoveisIndisponiveis().Count();
                var locatariosOcupados = _aluguelService.GetLocatariosOcupados().Count();
                var totalAlugueis = _aluguelService.GetCount();

                ViewBag.TotalImoveis = totalImoveis;
                ViewBag.ImoveisAlugados = imoveisAlugados;
                ViewBag.ImoveisDisponiveis = totalImoveis - imoveisAlugados;
                ViewBag.TotalLocatarios = totalLocatarios;
                ViewBag.LocatariosOcupados = locatariosOcupados;
                ViewBag.LocatariosDisponiveis = totalLocatarios - locatariosOcupados;
                ViewBag.TotalAlugueis = totalAlugueis;
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
