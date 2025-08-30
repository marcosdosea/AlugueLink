using System;
using System.Collections.Generic;

namespace Core;

public partial class Imovel
{
    public int Id { get; set; }

    public int IdLocador { get; set; }

    public string Cep { get; set; } = null!;

    public string Logradouro { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string? Complemento { get; set; }

    public string Bairro { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public int Quartos { get; set; }

    public int Banheiros { get; set; }

    public decimal Area { get; set; }

    public int VagasGaragem { get; set; }

    public decimal Valor { get; set; }

    public string Descricao { get; set; } = null!;

    public virtual ICollection<Aluguel> Aluguels { get; set; } = new List<Aluguel>();

    public virtual Locador IdLocadorNavigation { get; set; } = null!;

    public virtual ICollection<Manutencao> Manutencaos { get; set; } = new List<Manutencao>();
}
