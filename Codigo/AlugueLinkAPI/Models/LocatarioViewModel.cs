using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LocatarioViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome � obrigat�rio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Email � obrigat�rio")]
        [EmailAddress(ErrorMessage = "Email inv�lido")]
        [StringLength(100, ErrorMessage = "Email deve ter no m�ximo 100 caracteres")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefone principal � obrigat�rio")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 11 caracteres")]
        public string Telefone1 { get; set; } = null!;

        [Required(ErrorMessage = "Telefone secund�rio � obrigat�rio")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 11 caracteres")]
        public string Telefone2 { get; set; } = null!;

        [Required(ErrorMessage = "CPF � obrigat�rio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string Cpf { get; set; } = null!;

        [Required(ErrorMessage = "CEP � obrigat�rio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "CEP deve ter 8 caracteres")]
        public string Cep { get; set; } = null!;

        [Required(ErrorMessage = "Logradouro � obrigat�rio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Logradouro deve ter entre 5 e 100 caracteres")]
        public string Logradouro { get; set; } = null!;

        [Required(ErrorMessage = "N�mero � obrigat�rio")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "N�mero deve ter entre 1 e 5 caracteres")]
        public string Numero { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Complemento deve ter no m�ximo 50 caracteres")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro � obrigat�rio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bairro deve ter entre 2 e 50 caracteres")]
        public string Bairro { get; set; } = null!;

        [Required(ErrorMessage = "Cidade � obrigat�ria")]
        [StringLength(45, MinimumLength = 2, ErrorMessage = "Cidade deve ter entre 2 e 45 caracteres")]
        public string Cidade { get; set; } = null!;

        [Required(ErrorMessage = "Estado � obrigat�rio")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string Estado { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Profiss�o deve ter no m�ximo 100 caracteres")]
        public string? Profissao { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Renda deve ser entre 0 e 999999,99")]
        public decimal? Renda { get; set; }
    }
}