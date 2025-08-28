using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Aluguel
{
    public int Id { get; set; }

    public DateOnly? DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public string? Status { get; set; }

    public int ImovelId { get; set; }

    public int LocatárioId { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public virtual Imovel Imovel { get; set; } = null!;

    public virtual Locatário Locatário { get; set; } = null!;

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
