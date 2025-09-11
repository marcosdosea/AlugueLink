namespace Core.DTO
{
    public class PagamentoDto
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public string TipoPagamento { get; set; } = null!;
        public int Idaluguel { get; set; }
    }
}
