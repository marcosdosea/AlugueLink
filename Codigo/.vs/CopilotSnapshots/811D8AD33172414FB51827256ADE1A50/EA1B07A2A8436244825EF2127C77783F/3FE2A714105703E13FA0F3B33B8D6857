using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Manutencao
{
    public int Id { get; set; }

    public string? Descricao { get; set; }

    public DateOnly? DataSolicitacao { get; set; }

    public string? Status { get; set; }

    public int LocadorId { get; set; }

    public int LocatárioId { get; set; }

    public int DanoId { get; set; }

    public virtual Dano Dano { get; set; } = null!;

    public virtual Locador Locador { get; set; } = null!;

    public virtual Locatário Locatário { get; set; } = null!;
}
