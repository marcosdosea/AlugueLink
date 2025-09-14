using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LocadorViewModel
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

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 20 caracteres")]
        public string Telefone { get; set; } = null!;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string Cpf { get; set; } = null!;
    }
}