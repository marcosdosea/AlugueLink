using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Nome completo � obrigat�rio")]
        [StringLength(100, ErrorMessage = "O nome deve ter no m�ximo 100 caracteres")]
        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }
        
        public DateTime DataCadastro { get; set; }
        public DateTime? UltimoLogin { get; set; }
        
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Senha atual � obrigat�ria")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha � obrigat�ria")]
        [StringLength(100, ErrorMessage = "A nova senha deve ter pelo menos {2} e no m�ximo {1} caracteres.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "As senhas n�o coincidem.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}