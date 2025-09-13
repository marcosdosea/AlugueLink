using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers;

public class ExemploViaCepController : Controller
{
    private readonly IViaCepService _viaCepService;

    public ExemploViaCepController(IViaCepService viaCepService)
    {
        _viaCepService = viaCepService;
    }

    // GET: ExemploViaCep/ExemploUso
    public async Task<IActionResult> ExemploUso()
    {
        try
        {
            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync("01310-100");
            
            if (endereco != null)
            {
                var logradouro = endereco.Logradouro;
                var bairro = endereco.Bairro;
                var cidade = endereco.Localidade;
                var estado = endereco.Uf;
                
                var enderecoCompleto = new
                {
                    CEP = endereco.Cep,
                    Logradouro = endereco.Logradouro,
                    Complemento = endereco.Complemento,
                    Bairro = endereco.Bairro,
                    Cidade = endereco.Localidade,
                    Estado = endereco.Uf,
                    CodigoIBGE = endereco.Ibge,
                    DDD = endereco.Ddd
                };
                
                return Json(enderecoCompleto);
            }
            
            return BadRequest("CEP não encontrado");
        }
        catch (ServiceException ex)
        {
            return BadRequest(new { erro = true, mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = true, mensagem = "Erro interno do servidor" });
        }
    }

    // POST: ExemploViaCep/ValidarEnderecoAntesDeProcessar
    [HttpPost]
    public async Task<IActionResult> ValidarEnderecoAntesDeProcessar(string cep, string numero)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return BadRequest("CEP é obrigatório");
        }

        try
        {
            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync(cep);
            
            if (endereco != null)
            {
                var enderecoValidado = new
                {
                    CepOriginal = cep,
                    CepFormatado = endereco.Cep,
                    EnderecoCompleto = $"{endereco.Logradouro}, {numero}, {endereco.Bairro}, {endereco.Localidade}-{endereco.Uf}",
                    DadosCompletos = endereco
                };
                
                return Ok(new { sucesso = true, endereco = enderecoValidado });
            }
            
            return BadRequest("CEP não encontrado");
        }
        catch (ServiceException ex)
        {
            return BadRequest(new { sucesso = false, erro = ex.Message });
        }
    }
}