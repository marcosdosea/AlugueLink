using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AlugueLinkWEB.Areas.Identity.Data;

public class UsuarioIdentity : IdentityUser
{
    [Display(Name = "Nome Completo")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string? NomeCompleto { get; set; }

    [Display(Name = "Data de Nascimento")]
    [DataType(DataType.Date)]
    public DateTime? DataNascimento { get; set; }

    [Display(Name = "Data de Cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    [Display(Name = "Último Login")]
    public DateTime? UltimoLogin { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}

