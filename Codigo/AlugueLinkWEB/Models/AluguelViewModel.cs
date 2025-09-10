using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class AluguelViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "Data de início é obrigatória")]
        [DataType(DataType.Date)]
        public DateOnly? DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [Required(ErrorMessage = "Data de fim é obrigatória")]
        [DataType(DataType.Date)]
        public DateOnly? DataFim { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(20, ErrorMessage = "Status não pode ter mais de 20 caracteres")]
        public string? Status { get; set; }

        [Display(Name = "Data de Assinatura")]
        [Required(ErrorMessage = "Data de assinatura é obrigatória")]
        [DataType(DataType.Date)]
        public DateOnly? DataAssinatura { get; set; }

        [Display(Name = "Locatário")]
        [Required(ErrorMessage = "Locatário é obrigatório")]
        public int? IdLocatario { get; set; }

        [Display(Name = "Imóvel")]
        [Required(ErrorMessage = "Imóvel é obrigatório")]
        public int? IdImovel { get; set; }

        // Propriedades para exibição
        public string? LocatarioNome { get; set; }
        public string? ImovelEndereco { get; set; }
        public string? ImovelTipo { get; set; }
        public decimal? ImovelValor { get; set; }

        // Propriedade computed para exibir status amigável
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

        // Propriedade computed para exibir tipo de imóvel amigável
        public string ImovelTipoTexto
        {
            get
            {
                return ImovelTipo switch
                {
                    "A" => "Apartamento",
                    "C" => "Casa", 
                    "PC" => "Comercial",
                    "COM" => "Comercial", // fallback para possível variação
                    "casa" => "Casa",
                    "apartamento" => "Apartamento",
                    "comercial" => "Comercial",
                    _ => ImovelTipo ?? "Não informado"
                };
            }
        }

        // Para validação customizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (DataInicio.HasValue && DataFim.HasValue && DataInicio >= DataFim)
            {
                results.Add(new ValidationResult(
                    "A data de fim deve ser posterior à data de início.",
                    new[] { nameof(DataFim) }));
            }

            if (DataInicio.HasValue && DataAssinatura.HasValue && DataAssinatura > DataInicio)
            {
                results.Add(new ValidationResult(
                    "A data de assinatura não pode ser posterior à data de início.",
                    new[] { nameof(DataAssinatura) }));
            }

            return results;
        }
    }
}