using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PagamentoViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, 999999.99, ErrorMessage = "Valor deve ser entre 0,01 e 999999,99")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Data do pagamento é obrigatória")]
        [DataType(DataType.DateTime, ErrorMessage = "Data válida requerida")]
        public DateTime DataPagamento { get; set; }

        [Required(ErrorMessage = "Tipo de pagamento é obrigatório")]
        [StringLength(2, MinimumLength = 1, ErrorMessage = "Tipo de pagamento deve ter entre 1 e 2 caracteres")]
        public string TipoPagamento { get; set; } = null!;

        [Required(ErrorMessage = "Aluguel é obrigatório")]
        public int Idaluguel { get; set; }
    }
}