using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers;

/// <summary>
/// Exemplo de como usar o servi�o ViaCEP programaticamente em outros controllers
/// </summary>
public class ExemploViaCepController : Controller
{
    private readonly IViaCepService _viaCepService;

    public ExemploViaCepController(IViaCepService viaCepService)
    {
        _viaCepService = viaCepService;
    }

    /// <summary>
    /// Exemplo de action que usa o servi�o ViaCEP para validar e preencher endere�o
    /// </summary>
    public async Task<IActionResult> ExemploUso()
    {
        try
        {
            // Exemplo 1: Buscar endere�o por CEP
            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync("01310-100");
            
            if (endereco != null)
            {
                // Use os dados do endere�o
                var logradouro = endereco.Logradouro;
                var bairro = endereco.Bairro;
                var cidade = endereco.Localidade;
                var estado = endereco.Uf;
                
                // Exemplo: criar um objeto com o endere�o
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
            
            return BadRequest("CEP n�o encontrado");
        }
        catch (ServiceException ex)
        {
            // Tratar erro do servi�o
            return BadRequest(new { erro = true, mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            // Tratar erros gerais
            return StatusCode(500, new { erro = true, mensagem = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exemplo de valida��o de CEP antes de salvar um modelo
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ValidarEnderecoAntesDeProcessar(string cep, string numero)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return BadRequest("CEP � obrigat�rio");
        }

        try
        {
            // Buscar e validar o endere�o via ViaCEP
            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync(cep);
            
            if (endereco != null)
            {
                // Aqui voc� poderia:
                // 1. Criar um novo im�vel/locat�rio com o endere�o validado
                // 2. Atualizar um registro existente
                // 3. Fazer valida��es adicionais
                
                var enderecoValidado = new
                {
                    CepOriginal = cep,
                    CepFormatado = endereco.Cep,
                    EnderecoCompleto = $"{endereco.Logradouro}, {numero}, {endereco.Bairro}, {endereco.Localidade}-{endereco.Uf}",
                    DadosCompletos = endereco
                };
                
                return Ok(new { sucesso = true, endereco = enderecoValidado });
            }
            
            return BadRequest("CEP n�o encontrado");
        }
        catch (ServiceException ex)
        {
            return BadRequest(new { sucesso = false, erro = ex.Message });
        }
    }
}