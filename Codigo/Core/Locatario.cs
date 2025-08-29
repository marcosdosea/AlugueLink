using System;
using System.Collections.Generic;

namespace Core;

public partial class Locatario
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public virtual ICollection<Aluguel> Aluguels { get; set; } = new List<Aluguel>();

    public virtual ICollection<Locador> IdLocadors { get; set; } = new List<Locador>();
}
