using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Pagamento
{
    public int Id { get; set; }

    public decimal? Valor { get; set; }

    public DateOnly? DataPagamento { get; set; }

    public string? TipoPagamento { get; set; }

    public int AluguelId { get; set; }

    public virtual Aluguel Aluguel { get; set; } = null!;
}
