using Core.DTO;
using Core.Service;

namespace AlugueLinkWEB.Helpers;

public static class ViaCepHelper
{
    public static bool IsValidCep(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;
            
        var cleanCep = CleanCep(cep);
        return cleanCep.Length == 8 && cleanCep.All(char.IsDigit);
    }

    public static string CleanCep(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return string.Empty;
            
        return new string(cep.Where(char.IsDigit).ToArray());
    }

    public static string FormatCep(string? cep)
    {
        var cleanCep = CleanCep(cep);
        
        if (cleanCep.Length != 8)
            return cep ?? string.Empty;
            
        return $"{cleanCep.Substring(0, 5)}-{cleanCep.Substring(5, 3)}";
    }

    public static bool IsValidAddressSearchParams(string? uf, string? cidade, string? logradouro)
    {
        return !string.IsNullOrWhiteSpace(uf) &&
               !string.IsNullOrWhiteSpace(cidade) && cidade.Trim().Length >= 3 &&
               !string.IsNullOrWhiteSpace(logradouro) && logradouro.Trim().Length >= 3;
    }

    public static string FormatFullAddress(ViaCepDto endereco)
    {
        if (endereco == null)
            return string.Empty;

        var parts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(endereco.Logradouro))
            parts.Add(endereco.Logradouro);
            
        if (!string.IsNullOrWhiteSpace(endereco.Complemento))
            parts.Add(endereco.Complemento);
            
        if (!string.IsNullOrWhiteSpace(endereco.Bairro))
            parts.Add(endereco.Bairro);
            
        if (!string.IsNullOrWhiteSpace(endereco.Localidade))
            parts.Add(endereco.Localidade);
            
        if (!string.IsNullOrWhiteSpace(endereco.Uf))
            parts.Add(endereco.Uf);

        return string.Join(", ", parts);
    }

    public static string GetViaCepInitScript(Dictionary<string, string>? elementIds = null)
    {
        var defaultIds = new Dictionary<string, string>
        {
            ["cep"] = "Cep",
            ["logradouro"] = "Logradouro", 
            ["complemento"] = "Complemento",
            ["bairro"] = "Bairro",
            ["cidade"] = "Cidade",
            ["estado"] = "Estado"
        };

        var ids = elementIds ?? defaultIds;
        var idsJson = System.Text.Json.JsonSerializer.Serialize(ids);

        return $@"
document.addEventListener('DOMContentLoaded', function() {{
    if (typeof ViaCepUtil !== 'undefined') {{
        ViaCepUtil.init({idsJson});
    }}
}});";
    }
}