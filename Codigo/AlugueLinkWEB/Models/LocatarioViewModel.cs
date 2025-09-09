using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class LocatarioViewModel //: IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome não pode ter mais de 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email não pode ter mais de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone Principal")]
        [Required(ErrorMessage = "Telefone principal é obrigatório")]
        [StringLength(11, ErrorMessage = "Telefone deve ter 11 caracteres")]
        public string Telefone1 { get; set; } = string.Empty;

        [Display(Name = "Telefone Secundário")]
        [StringLength(11, ErrorMessage = "Telefone deve ter 11 caracteres")]
        public string? Telefone2 { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "CEP é obrigatório")]
        [StringLength(8, ErrorMessage = "CEP deve ter 8 caracteres")]
        public string Cep { get; set; } = string.Empty;

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Logradouro é obrigatório")]
        [StringLength(100, ErrorMessage = "Logradouro não pode ter mais de 100 caracteres")]
        public string Logradouro { get; set; } = string.Empty;

        [Display(Name = "Número")]
        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(5, ErrorMessage = "Número não pode ter mais de 5 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Complemento não pode ter mais de 50 caracteres")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Bairro é obrigatório")]
        [StringLength(50, ErrorMessage = "Bairro não pode ter mais de 50 caracteres")]
        public string Bairro { get; set; } = string.Empty;

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(45, ErrorMessage = "Cidade não pode ter mais de 45 caracteres")]
        public string Cidade { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Profissão")]
        [StringLength(100, ErrorMessage = "Profissão não pode ter mais de 100 caracteres")]
        public string? Profissao { get; set; }

        [Display(Name = "Renda")]
        [Range(0.01, 999999.99, ErrorMessage = "Renda deve ser um valor entre R$ 0,01 e R$ 999.999,99")]
        [DataType(DataType.Currency)]
        public decimal? Renda { get; set; }

        // Propriedades para status de aluguel
        public bool IsOcupado { get; set; }
        public string? StatusTexto => IsOcupado ? "Ocupado" : "Disponível";
        public string? ImovelAtual { get; set; }
        public DateOnly? DataInicioAluguel { get; set; }
        public DateOnly? DataFimAluguel { get; set; }

        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Validações customizadas podem ser adicionadas aqui

            return results;
        }*/
    }
}