using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class LocadorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome � obrigat�rio")]
        [StringLength(50, ErrorMessage = "Nome n�o pode ter mais de 50 caracteres")]
        public string? Nome { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail � obrigat�rio")]
        [EmailAddress(ErrorMessage = "E-mail inv�lido")]
        [StringLength(100, ErrorMessage = "E-mail n�o pode ter mais de 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Telefone � obrigat�rio")]
        [Phone(ErrorMessage = "Telefone inv�lido")]
        [StringLength(15, ErrorMessage = "Telefone n�o pode ter mais de 15 caracteres")]
        public string? Telefone { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "CPF � obrigat�rio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string? Cpf { get; set; }
    }
}