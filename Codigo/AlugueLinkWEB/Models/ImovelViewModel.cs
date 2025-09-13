using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AlugueLinkWEB.Models
{
    public class ImovelViewModel
    {
        public int Id { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "CEP é obrigatório")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve ter o formato 00000-000 ou 00000000")]
        public string? Cep { get; set; }

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Logradouro é obrigatório")]
        [StringLength(100, ErrorMessage = "Logradouro não pode ter mais de 100 caracteres")]
        public string? Logradouro { get; set; }

        [Display(Name = "Número")]
        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(5, ErrorMessage = "Número não pode ter mais de 5 caracteres")]
        public string? Numero { get; set; }

        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Complemento não pode ter mais de 50 caracteres")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Bairro é obrigatório")]
        [StringLength(50, ErrorMessage = "Bairro não pode ter mais de 50 caracteres")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(45, ErrorMessage = "Cidade não pode ter mais de 45 caracteres")]
        public string? Cidade { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string? Estado { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Tipo de imóvel é obrigatório")]
        public string? Tipo { get; set; }

        [Display(Name = "Quartos")]
        [Range(0, 50, ErrorMessage = "Número de quartos deve estar entre 0 e 50")]
        public int? Quartos { get; set; }

        [Display(Name = "Banheiros")]
        [Range(0, 50, ErrorMessage = "Número de banheiros deve estar entre 0 e 50")]
        public int? Banheiros { get; set; }

        [Display(Name = "Área (m²)")]
        [Required(ErrorMessage = "Área é obrigatória")]
        [RegularExpression(@"^\d+,\d{1,2}$", ErrorMessage = "Área deve estar no formato 0,0 ou 0,00")]        
        public string? AreaStr { get; set; }

        public decimal? Area { get; set; }

        [Display(Name = "Vagas Garagem")]
        [Range(0, 50, ErrorMessage = "Número de vagas deve estar entre 0 e 50")]
        public int? VagasGaragem { get; set; }

        [Display(Name = "Valor (R$)")]
        [Required(ErrorMessage = "Valor do aluguel é obrigatório")]
        [RegularExpression(@"^\d+,\d{1,2}$", ErrorMessage = "Valor deve estar no formato 0,0 ou 0,00")]        
        public string? ValorStr { get; set; }

        public decimal? Valor { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200, ErrorMessage = "Descrição não pode ter mais de 200 caracteres")]
        public string? Descricao { get; set; }

        [Display(Name = "Locador")]
        public int? LocadorId { get; set; }

        public string? LocadorNome { get; set; }

        public bool IsAlugado { get; set; }
        public string? StatusTexto => IsAlugado ? "Alugado" : "Disponível";
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
                    _ => Tipo ?? "Não informado"
                };
            }
        }

        public string? CepLimpo => string.IsNullOrWhiteSpace(Cep) ? null : Regex.Replace(Cep, @"\D", "");
    }
}