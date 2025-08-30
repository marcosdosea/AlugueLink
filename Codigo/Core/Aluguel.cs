using System;
using System.Collections.Generic;

namespace Core;

public partial class Aluguel
{
    public int Id { get; set; }

    public DateOnly DataInicio { get; set; }

    public DateOnly DataFim { get; set; }

    public string? Status { get; set; }

    public DateOnly DataAssinatura { get; set; }

    public int Idlocatario { get; set; }

    public int Idimovel { get; set; }

    public virtual Imovel IdimovelNavigation { get; set; } = null!;

    public virtual Locatario IdlocatarioNavigation { get; set; } = null!;

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
