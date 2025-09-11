using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace AlugueLinkWEB.Controllers;

/// <summary>
/// Exemplo de como usar o serviço ViaCEP programaticamente em outros controllers
/// </summary>
public class ExemploViaCepController : Controller
{
    private readonly IViaCepService _viaCepService;

    public ExemploViaCepController(IViaCepService viaCepService)
    {
        _viaCepService = viaCepService;
    }

    /// <summary>
    /// Exemplo de action que usa o serviço ViaCEP para validar e preencher endereço
    /// </summary>
    public async Task<IActionResult> ExemploUso()
    {
        try
        {
            // Exemplo 1: Buscar endereço por CEP
            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync("01310-100");
            
            if (endereco != null)
            {
                // Use os dados do endereço
                var logradouro = endereco.Logradouro;
                var bairro = endereco.Bairro;
                var cidade = endereco.Localidade;
                var estado = endereco.Uf;
                
                // Exemplo: criar um objeto com o endereço
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
            // Tratar erro do serviço
            return BadRequest(new { erro = true, mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            // Tratar erros gerais
            return StatusCode(500, new { erro = true, mensagem = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exemplo de validação de CEP antes de salvar um modelo
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ValidarEnderecoAntesDeProcessar(string cep, string numero)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return BadRequest("CEP é obrigatório");
        }

        try
        {
            // Buscar e validar o endereço via ViaCEP
            var endereco = await _viaCepService.BuscarEnderecoPorCepAsync(cep);
            
            if (endereco != null)
            {
                // Aqui você poderia:
                // 1. Criar um novo imóvel/locatário com o endereço validado
                // 2. Atualizar um registro existente
                // 3. Fazer validações adicionais
                
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