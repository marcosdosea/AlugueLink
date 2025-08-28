using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Manutencao
{
    public int Id { get; set; }

    public string Descricao { get; set; } = null!;

    public DateOnly DataSolicitacao { get; set; }

    public string? Status { get; set; }

    public decimal Valor { get; set; }

    public int ImovelId { get; set; }

    public int ImovelLocadorId { get; set; }

    public virtual Imovel Imovel { get; set; } = null!;
}
