using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ViaCepController : ControllerBase
{
    private readonly IViaCepService _viaCepService;

    public ViaCepController(IViaCepService viaCepService)
    {
        _viaCepService = viaCepService;
    }

    // GET: api/ViaCep/{cep}
    [HttpGet("{cep}")]
    public async Task<IActionResult> BuscarEnderecoPorCep(string cep)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                return BadRequest(new { erro = true, mensagem = "CEP é obrigatório." });
            }

            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync(cep);
            
            if (endereco == null)
            {
                return NotFound(new { erro = true, mensagem = "CEP não encontrado." });
            }

            return Ok(endereco);
        }
        catch (ServiceException ex)
        {
            return BadRequest(new { erro = true, mensagem = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { erro = true, mensagem = "Erro interno do servidor." });
        }
    }
}