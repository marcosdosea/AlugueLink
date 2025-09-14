using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LocatarioViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefone principal é obrigatório")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 11 caracteres")]
        public string Telefone1 { get; set; } = null!;

        [Required(ErrorMessage = "Telefone secundário é obrigatório")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 11 caracteres")]
        public string Telefone2 { get; set; } = null!;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string Cpf { get; set; } = null!;

        [Required(ErrorMessage = "CEP é obrigatório")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "CEP deve ter 8 caracteres")]
        public string Cep { get; set; } = null!;

        [Required(ErrorMessage = "Logradouro é obrigatório")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Logradouro deve ter entre 5 e 100 caracteres")]
        public string Logradouro { get; set; } = null!;

        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "Número deve ter entre 1 e 5 caracteres")]
        public string Numero { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Complemento deve ter no máximo 50 caracteres")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bairro deve ter entre 2 e 50 caracteres")]
        public string Bairro { get; set; } = null!;

        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(45, MinimumLength = 2, ErrorMessage = "Cidade deve ter entre 2 e 45 caracteres")]
        public string Cidade { get; set; } = null!;

        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string Estado { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Profissão deve ter no máximo 100 caracteres")]
        public string? Profissao { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Renda deve ser entre 0 e 999999,99")]
        public decimal? Renda { get; set; }
    }
}