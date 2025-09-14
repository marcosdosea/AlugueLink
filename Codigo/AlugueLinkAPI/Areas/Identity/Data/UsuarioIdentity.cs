using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AlugueLinkAPI.Areas.Identity.Data
{
    public class UsuarioIdentity : IdentityUser
    {
        [Display(Name = "Nome Completo")]
        [StringLength(100, ErrorMessage = "O nome deve ter no m�ximo 100 caracteres")]
        public string? NomeCompleto { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Display(Name = "�ltimo Login")]
        public DateTime? UltimoLogin { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}