using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class LocadorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(50, ErrorMessage = "Nome não pode ter mais de 50 caracteres")]
        public string? Nome { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(100, ErrorMessage = "E-mail não pode ter mais de 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(15, ErrorMessage = "Telefone não pode ter mais de 15 caracteres")]
        public string? Telefone { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string? Cpf { get; set; }
    }
}