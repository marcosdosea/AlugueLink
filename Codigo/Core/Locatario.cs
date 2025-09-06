using System;
using System.Collections.Generic;

namespace Core;

public partial class Locatario
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone1 { get; set; } = null!;

    public string Telefone2 { get; set; } = null!; // obrigatório novamente

    public string Cpf { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public string Logradouro { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string? Complemento { get; set; }

    public string Bairro { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? Profissao { get; set; }

    public decimal? Renda { get; set; }

    public virtual ICollection<Aluguel> Aluguels { get; set; } = new List<Aluguel>();

    public virtual ICollection<Locador> Idlocadors { get; set; } = new List<Locador>();
}
