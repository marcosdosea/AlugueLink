using System.ComponentModel.DataAnnotations;
using Util;

namespace AlugueLinkWEB.Models
{
    public class LocadorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome � obrigat�rio")]
        [StringLength(50, ErrorMessage = "Nome n�o pode ter mais de 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail � obrigat�rio")]
        [EmailAddress(ErrorMessage = "E-mail inv�lido")]
        [StringLength(100, ErrorMessage = "E-mail n�o pode ter mais de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Telefone � obrigat�rio")]
        [StringLength(15, ErrorMessage = "Telefone n�o pode ter mais de 15 caracteres")]
        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "CPF � obrigat�rio")]
        [CPF(ErrorMessage = "CPF inv�lido")]
        [StringLength(14, ErrorMessage = "CPF deve ter no m�ximo 14 caracteres")]
        public string Cpf { get; set; } = string.Empty;
    }
}