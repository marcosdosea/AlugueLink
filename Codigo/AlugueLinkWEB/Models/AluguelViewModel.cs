using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class AluguelViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Data de In�cio")]
        [Required(ErrorMessage = "Data de in�cio � obrigat�ria")]
        [DataType(DataType.Date)]
        public DateOnly? DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [Required(ErrorMessage = "Data de fim � obrigat�ria")]
        [DataType(DataType.Date)]
        public DateOnly? DataFim { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status � obrigat�rio")]
        [StringLength(20, ErrorMessage = "Status n�o pode ter mais de 20 caracteres")]
        public string? Status { get; set; }

        [Display(Name = "Data de Assinatura")]
        [Required(ErrorMessage = "Data de assinatura � obrigat�ria")]
        [DataType(DataType.Date)]
        public DateOnly? DataAssinatura { get; set; }

        [Display(Name = "Locat�rio")]
        [Required(ErrorMessage = "Locat�rio � obrigat�rio")]
        public int? IdLocatario { get; set; }

        [Display(Name = "Im�vel")]
        [Required(ErrorMessage = "Im�vel � obrigat�rio")]
        public int? IdImovel { get; set; }

        // Propriedades para exibi��o
        public string? LocatarioNome { get; set; }
        public string? ImovelEndereco { get; set; }
        public string? ImovelTipo { get; set; }
        public decimal? ImovelValor { get; set; }

        // Propriedade computed para exibir status amig�vel
        public string StatusTexto
        {
            get
            {
                return Status switch
                {
                    "A" => "Ativo",
                    "F" => "Finalizado",
                    "P" => "Pendente",
                    _ => Status ?? "Desconhecido"
                };
            }
        }

        // Propriedade computed para exibir tipo de im�vel amig�vel
        public string ImovelTipoTexto
        {
            get
            {
                return ImovelTipo switch
                {
                    "A" => "Apartamento",
                    "C" => "Casa", 
                    "PC" => "Comercial",
                    "COM" => "Comercial", // fallback para poss�vel varia��o
                    "casa" => "Casa",
                    "apartamento" => "Apartamento",
                    "comercial" => "Comercial",
                    _ => ImovelTipo ?? "N�o informado"
                };
            }
        }

        // Para valida��o customizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (DataInicio.HasValue && DataFim.HasValue && DataInicio >= DataFim)
            {
                results.Add(new ValidationResult(
                    "A data de fim deve ser posterior � data de in�cio.",
                    new[] { nameof(DataFim) }));
            }

            if (DataInicio.HasValue && DataAssinatura.HasValue && DataAssinatura > DataInicio)
            {
                results.Add(new ValidationResult(
                    "A data de assinatura n�o pode ser posterior � data de in�cio.",
                    new[] { nameof(DataAssinatura) }));
            }

            return results;
        }
    }
}