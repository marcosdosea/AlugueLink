using System.ComponentModel.DataAnnotations;
using Util;

namespace AlugueLinkWEB.Models
{
    public class LocadorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(50, ErrorMessage = "Nome não pode ter mais de 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(100, ErrorMessage = "E-mail não pode ter mais de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(15, ErrorMessage = "Telefone não pode ter mais de 15 caracteres")]
        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "CPF é obrigatório")]
        [CPF(ErrorMessage = "CPF inválido")]
        [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        public string Cpf { get; set; } = string.Empty;
    }
}