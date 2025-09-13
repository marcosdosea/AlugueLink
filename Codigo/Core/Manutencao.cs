using System;
using System.Collections.Generic;

namespace Core;

public partial class Manutencao
{
    public int Id { get; set; }

    public string Descricao { get; set; } = null!;

    public DateTime DataSolicitacao { get; set; }

    // P - PEDIDO REALIZADO, A - ATENDIDA, C - CANCELADA
    public string Status { get; set; } = null!;

    public decimal Valor { get; set; }

    public int Idimovel { get; set; }

    public virtual Imovel IdimovelNavigation { get; set; } = null!;
}
