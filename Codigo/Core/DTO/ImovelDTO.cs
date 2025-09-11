namespace Core.DTO
{
    public class ImovelDto
    {
        public int Id { get; set; }
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
        public int IdLocador { get; set; }
    }
}