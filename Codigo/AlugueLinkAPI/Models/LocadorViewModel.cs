using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LocadorViewModel
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

        [Required(ErrorMessage = "Telefone � obrigat�rio")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 20 caracteres")]
        public string Telefone { get; set; } = null!;

        [Required(ErrorMessage = "CPF � obrigat�rio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string Cpf { get; set; } = null!;
    }
}