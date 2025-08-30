using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class LocatarioViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome � obrigat�rio")]
        [StringLength(100, ErrorMessage = "Nome n�o pode ter mais de 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email � obrigat�rio")]
        [EmailAddress(ErrorMessage = "Email inv�lido")]
        [StringLength(100, ErrorMessage = "Email n�o pode ter mais de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone Principal")]
        [Required(ErrorMessage = "Telefone principal � obrigat�rio")]
        [StringLength(11, ErrorMessage = "Telefone deve ter at� 11 caracteres")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas n�meros (10 ou 11 d�gitos)")]
        public string Telefone1 { get; set; } = string.Empty;

        [Display(Name = "Telefone Secund�rio")]
        [StringLength(11, ErrorMessage = "Telefone deve ter at� 11 caracteres")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone deve conter apenas n�meros (10 ou 11 d�gitos)")]
        public string? Telefone2 { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "CPF � obrigat�rio")]
        [StringLength(11, ErrorMessage = "CPF deve ter 11 caracteres")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas n�meros")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "CEP � obrigat�rio")]
        [StringLength(8, ErrorMessage = "CEP deve ter 8 caracteres")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter apenas n�meros")]
        public string Cep { get; set; } = string.Empty;

        [Display(Name = "Logradouro")]
        [Required(ErrorMessage = "Logradouro � obrigat�rio")]
        [StringLength(100, ErrorMessage = "Logradouro n�o pode ter mais de 100 caracteres")]
        public string Logradouro { get; set; } = string.Empty;

        [Display(Name = "N�mero")]
        [Required(ErrorMessage = "N�mero � obrigat�rio")]
        [StringLength(5, ErrorMessage = "N�mero n�o pode ter mais de 5 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Complemento n�o pode ter mais de 50 caracteres")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [Required(ErrorMessage = "Bairro � obrigat�rio")]
        [StringLength(50, ErrorMessage = "Bairro n�o pode ter mais de 50 caracteres")]
        public string Bairro { get; set; } = string.Empty;

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Cidade � obrigat�ria")]
        [StringLength(45, ErrorMessage = "Cidade n�o pode ter mais de 45 caracteres")]
        public string Cidade { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Estado � obrigat�rio")]
        [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Profiss�o")]
        [StringLength(100, ErrorMessage = "Profiss�o n�o pode ter mais de 100 caracteres")]
        public string? Profissao { get; set; }

        [Display(Name = "Renda (R$)")]
        [Range(0, 999999.99, ErrorMessage = "Renda deve estar entre R$ 0,00 e R$ 999.999,99")]
        [DataType(DataType.Currency)]
        public decimal? Renda { get; set; }
    }
}