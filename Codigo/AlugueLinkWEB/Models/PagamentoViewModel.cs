using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class PagamentoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Valor (R$)")]
        public string? ValorStr { get; set; }

        public decimal? Valor { get; set; }

        [Display(Name = "Data do Pagamento")]
        [Required(ErrorMessage = "Data do pagamento � obrigat�ria")]
        [DataType(DataType.Date)]
        public DateOnly? DataPagamento { get; set; }

        [Display(Name = "Hora do Pagamento")]
        [Required(ErrorMessage = "Hora do pagamento � obrigat�ria")]
        [DataType(DataType.Time)]
        public TimeOnly? HoraPagamento { get; set; }

        [Display(Name = "Tipo de Pagamento")]
        [Required(ErrorMessage = "Tipo de pagamento � obrigat�rio")]
        public string? TipoPagamento { get; set; }

        [Display(Name = "Aluguel")]
        [Required(ErrorMessage = "Aluguel � obrigat�rio")]
        public int? AluguelId { get; set; }

        // Propriedades para exibi��o
        public string? ImovelEndereco { get; set; }
        public string? LocatarioNome { get; set; }
        public decimal? ValorAluguel { get; set; }
        public DateOnly? DataInicioAluguel { get; set; }
        public DateOnly? DataFimAluguel { get; set; }
        public string? StatusAluguel { get; set; }

        // Propriedade computada para combinar data e hora
        public DateTime? DataHoraPagamento
        {
            get
            {
                if (!DataPagamento.HasValue || !HoraPagamento.HasValue)
                    return null;
                
                return DataPagamento.Value.ToDateTime(HoraPagamento.Value);
            }
            set
            {
                if (value.HasValue)
                {
                    DataPagamento = DateOnly.FromDateTime(value.Value);
                    HoraPagamento = TimeOnly.FromDateTime(value.Value);
                }
                else
                {
                    DataPagamento = null;
                    HoraPagamento = null;
                }
            }
        }

        public string TipoPagamentoTexto
        {
            get
            {
                return TipoPagamento switch
                {
                    "CD" => "Cart�o de D�bito",
                    "CC" => "Cart�o de Cr�dito",
                    "P" => "PIX",
                    "B" => "Boleto",
                    _ => TipoPagamento ?? "N�o informado"
                };
            }
        }

        public string StatusPagamento
        {
            get
            {
                if (!DataHoraPagamento.HasValue) return "Pendente";
                
                var agora = DateTime.Now;
                if (DataHoraPagamento.Value <= agora)
                    return "Pago";
                
                return "Agendado";
            }
        }

        public string StatusCssClass
        {
            get
            {
                return StatusPagamento switch
                {
                    "Pago" => "text-success",
                    "Pendente" => "text-warning",
                    "Agendado" => "text-info",
                    _ => "text-muted"
                };
            }
        }
    }
}