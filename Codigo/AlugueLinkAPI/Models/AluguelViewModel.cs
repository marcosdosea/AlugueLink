using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AluguelViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Data de in�cio � obrigat�ria")]
        [DataType(DataType.Date, ErrorMessage = "Data v�lida requerida")]
        public DateOnly DataInicio { get; set; }

        [Required(ErrorMessage = "Data de fim � obrigat�ria")]
        [DataType(DataType.Date, ErrorMessage = "Data v�lida requerida")]
        public DateOnly DataFim { get; set; }

        [StringLength(1, ErrorMessage = "Status deve ter 1 caractere")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Data de assinatura � obrigat�ria")]
        [DataType(DataType.Date, ErrorMessage = "Data v�lida requerida")]
        public DateOnly DataAssinatura { get; set; }

        [Required(ErrorMessage = "Locat�rio � obrigat�rio")]
        public int Idlocatario { get; set; }

        [Required(ErrorMessage = "Im�vel � obrigat�rio")]
        public int Idimovel { get; set; }
    }
}