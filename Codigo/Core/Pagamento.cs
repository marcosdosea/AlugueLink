using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public int Id { get; set; }

    public decimal Valor { get; set; }

    public DateTime DataPagamento { get; set; }

    public string TipoPagamento { get; set; } = null!;

    public int Idaluguel { get; set; }

    public virtual Aluguel IdaluguelNavigation { get; set; } = null!;
}
