namespace Core.DTO
{
    public class PagamentoDTO
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateOnly DataPagamento { get; set; }
        public string TipoPagamento { get; set; } = string.Empty;
        public int IdAluguel { get; set; }
    }
}