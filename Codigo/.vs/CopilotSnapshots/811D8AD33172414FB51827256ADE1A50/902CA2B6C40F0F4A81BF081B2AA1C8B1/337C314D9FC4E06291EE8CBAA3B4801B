using System;
using System.Collections.Generic;

namespace Core;

public partial class Locador
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public virtual ICollection<Imovel> Imovels { get; set; } = new List<Imovel>();

    public virtual ICollection<Locatario> IdLocatarios { get; set; } = new List<Locatario>();
}
