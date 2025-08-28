using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Dano
{
    public int Id { get; set; }

    public string? Descricao { get; set; }

    public decimal? Valor { get; set; }

    public virtual ICollection<Manutencao> Manutencaos { get; set; } = new List<Manutencao>();
}
