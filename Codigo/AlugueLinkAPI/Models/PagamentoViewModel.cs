using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PagamentoViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Valor � obrigat�rio")]
        [Range(0.01, 999999.99, ErrorMessage = "Valor deve ser entre 0,01 e 999999,99")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Data do pagamento � obrigat�ria")]
        [DataType(DataType.DateTime, ErrorMessage = "Data v�lida requerida")]
        public DateTime DataPagamento { get; set; }

        [Required(ErrorMessage = "Tipo de pagamento � obrigat�rio")]
        [StringLength(2, MinimumLength = 1, ErrorMessage = "Tipo de pagamento deve ter entre 1 e 2 caracteres")]
        public string TipoPagamento { get; set; } = null!;

        [Required(ErrorMessage = "Aluguel � obrigat�rio")]
        public int Idaluguel { get; set; }
    }
}