using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AluguelViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Data de início é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data válida requerida")]
        public DateOnly DataInicio { get; set; }

        [Required(ErrorMessage = "Data de fim é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data válida requerida")]
        public DateOnly DataFim { get; set; }

        [StringLength(1, ErrorMessage = "Status deve ter 1 caractere")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Data de assinatura é obrigatória")]
        [DataType(DataType.Date, ErrorMessage = "Data válida requerida")]
        public DateOnly DataAssinatura { get; set; }

        [Required(ErrorMessage = "Locatário é obrigatório")]
        public int Idlocatario { get; set; }

        [Required(ErrorMessage = "Imóvel é obrigatório")]
        public int Idimovel { get; set; }
    }
}