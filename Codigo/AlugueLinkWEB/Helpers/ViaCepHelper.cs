using Core.DTO;
using Core.Service;

namespace AlugueLinkWEB.Helpers;

/// <summary>
/// Helper class for ViaCEP integration utilities
/// </summary>
public static class ViaCepHelper
{
    /// <summary>
    /// Validates a Brazilian CEP format
    /// </summary>
    /// <param name="cep">CEP to validate</param>
    /// <returns>True if CEP is valid</returns>
    public static bool IsValidCep(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;
            
        var cleanCep = CleanCep(cep);
        return cleanCep.Length == 8 && cleanCep.All(char.IsDigit);
    }

    /// <summary>
    /// Cleans CEP by removing non-numeric characters
    /// </summary>
    /// <param name="cep">CEP to clean</param>
    /// <returns>Clean CEP with only numbers</returns>
    public static string CleanCep(string? cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return string.Empty;
            
        return new string(cep.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Formats CEP for display (00000-000)
    /// </summary>
    /// <param name="cep">CEP to format</param>
    /// <returns>Formatted CEP</returns>
    public static string FormatCep(string? cep)
    {
        var cleanCep = CleanCep(cep);
        
        if (cleanCep.Length != 8)
            return cep ?? string.Empty;
            
        return $"{cleanCep.Substring(0, 5)}-{cleanCep.Substring(5, 3)}";
    }

    /// <summary>
    /// Gets the JavaScript initialization script for ViaCEP
    /// </summary>
    /// <param name="elementIds">Dictionary of field IDs</param>
    /// <returns>JavaScript initialization code</returns>
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