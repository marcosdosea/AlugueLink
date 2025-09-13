using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AlugueLinkWEB.Models
{
    public class ImovelViewModel
    {
        public int Id { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "CEP � obrigat�rio")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve ter o formato 00000-000 ou 00000000")]
        public string? Cep { get; set; }

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Logradouro � obrigat�rio")]
        [StringLength(100, ErrorMessage = "Logradouro n�o pode ter mais de 100 caracteres")]
        public string? Logradouro { get; set; }

        [Display(Name = "N�mero")]
        [Required(ErrorMessage = "N�mero � obrigat�rio")]
        [StringLength(5, ErrorMessage = "N�mero n�o pode ter mais de 5 caracteres")]
        public string? Numero { get; set; }

        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Complemento n�o pode ter mais de 50 caracteres")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Bairro � obrigat�rio")]
        [StringLength(50, ErrorMessage = "Bairro n�o pode ter mais de 50 caracteres")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Cidade � obrigat�ria")]
        [StringLength(45, ErrorMessage = "Cidade n�o pode ter mais de 45 caracteres")]
        public string? Cidade { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Estado � obrigat�rio")]
        [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string? Estado { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Tipo de im�vel � obrigat�rio")]
        public string? Tipo { get; set; }

        [Display(Name = "Quartos")]
        [Range(0, 50, ErrorMessage = "N�mero de quartos deve estar entre 0 e 50")]
        public int? Quartos { get; set; }

        [Display(Name = "Banheiros")]
        [Range(0, 50, ErrorMessage = "N�mero de banheiros deve estar entre 0 e 50")]
        public int? Banheiros { get; set; }

        [Display(Name = "�rea (m�)")]
        [Required(ErrorMessage = "�rea � obrigat�ria")]
        [RegularExpression(@"^\d+,\d{1,2}$", ErrorMessage = "�rea deve estar no formato 0,0 ou 0,00")]        
        public string? AreaStr { get; set; }

        public decimal? Area { get; set; }

        [Display(Name = "Vagas Garagem")]
        [Range(0, 50, ErrorMessage = "N�mero de vagas deve estar entre 0 e 50")]
        public int? VagasGaragem { get; set; }

        [Display(Name = "Valor (R$)")]
        [Required(ErrorMessage = "Valor do aluguel � obrigat�rio")]
        [RegularExpression(@"^\d+,\d{1,2}$", ErrorMessage = "Valor deve estar no formato 0,0 ou 0,00")]        
        public string? ValorStr { get; set; }

        public decimal? Valor { get; set; }

        [Display(Name = "Descri��o")]
        [Required(ErrorMessage = "Descri��o � obrigat�ria")]
        [StringLength(200, ErrorMessage = "Descri��o n�o pode ter mais de 200 caracteres")]
        public string? Descricao { get; set; }

        [Display(Name = "Locador")]
        public int? LocadorId { get; set; }

        public string? LocadorNome { get; set; }

        public bool IsAlugado { get; set; }
        public string? StatusTexto => IsAlugado ? "Alugado" : "Dispon�vel";
        public string? InquilinoAtual { get; set; }
        public DateOnly? DataInicioAluguel { get; set; }
        public DateOnly? DataFimAluguel { get; set; }

        public bool IsComercial => !string.IsNullOrEmpty(Tipo) &&
                                   Tipo.Equals("comercial", StringComparison.OrdinalIgnoreCase);

        public string TipoTexto
        {
            get
            {
                return Tipo switch
                {
                    "C" => "Casa",
                    "A" => "Apartamento", 
                    "COM" => "Comercial",
                    "PC" => "Comercial",
                    "casa" => "Casa",
                    "apartamento" => "Apartamento",
                    "comercial" => "Comercial",
                    _ => Tipo ?? "N�o informado"
                };
            }
        }

        public string? CepLimpo => string.IsNullOrWhiteSpace(Cep) ? null : Regex.Replace(Cep, @"\D", "");
    }
}