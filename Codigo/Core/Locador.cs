using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Locador
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? PasswordHash { get; set; }

    public string? Cpf { get; set; }

    public virtual ICollection<Imovel> Imovels { get; set; } = new List<Imovel>();

    public virtual ICollection<Manutencao> Manutencaos { get; set; } = new List<Manutencao>();
}
