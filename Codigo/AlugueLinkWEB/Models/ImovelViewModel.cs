using System.ComponentModel.DataAnnotations;

namespace AlugueLinkWEB.Models
{
    public class ImovelViewModel
    {
        public int Id { get; set; }

        [Display(Name = "CEP")]
        [StringLength(8, ErrorMessage = "CEP deve ter 8 caracteres")]
        public string? Cep { get; set; }

        [Display(Name = "Logradouro")]
        [StringLength(100, ErrorMessage = "Logradouro n�o pode ter mais de 100 caracteres")]
        public string? Logradouro { get; set; }

        [Display(Name = "N�mero")]
        [StringLength(5, ErrorMessage = "N�mero n�o pode ter mais de 5 caracteres")]
        public string? Numero { get; set; }

        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Complemento n�o pode ter mais de 50 caracteres")]
        public string? Complemento { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(50, ErrorMessage = "Bairro n�o pode ter mais de 50 caracteres")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [StringLength(45, ErrorMessage = "Cidade n�o pode ter mais de 45 caracteres")]
        public string? Cidade { get; set; }

        [Display(Name = "Estado")]
        [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string? Estado { get; set; }

        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }

        [Display(Name = "Quartos")]
        [Range(0, 10, ErrorMessage = "N�mero de quartos deve estar entre 0 e 10")]
        public int? Quartos { get; set; }

        [Display(Name = "Banheiros")]
        [Range(0, 10, ErrorMessage = "N�mero de banheiros deve estar entre 0 e 10")]
        public int? Banheiros { get; set; }

        [Display(Name = "�rea (m�)")]
        [Range(0, double.MaxValue, ErrorMessage = "�rea deve ser um valor positivo")]
        public decimal? Area { get; set; }

        [Display(Name = "Vagas de Garagem")]
        [Range(0, 10, ErrorMessage = "N�mero de vagas deve estar entre 0 e 10")]
        public int? VagasGaragem { get; set; }

        [Display(Name = "Valor (R$)")]
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        [DataType(DataType.Currency)]
        public decimal? Valor { get; set; }

        [Display(Name = "Descri��o")]
        public string? Descricao { get; set; }

        [Display(Name = "Locador")]
        public int LocadorId { get; set; }

        public string? LocadorNome { get; set; }
    }
}