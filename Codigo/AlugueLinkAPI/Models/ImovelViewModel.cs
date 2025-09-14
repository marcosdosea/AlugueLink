using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ImovelViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Locador é obrigatório")]
        public int IdLocador { get; set; }

        [Required(ErrorMessage = "CEP é obrigatório")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "CEP deve ter 8 caracteres")]
        public string Cep { get; set; } = null!;

        [Required(ErrorMessage = "Logradouro é obrigatório")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Logradouro deve ter entre 5 e 100 caracteres")]
        public string Logradouro { get; set; } = null!;

        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "Número deve ter entre 1 e 5 caracteres")]
        public string Numero { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Complemento deve ter no máximo 50 caracteres")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bairro deve ter entre 2 e 50 caracteres")]
        public string Bairro { get; set; } = null!;

        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(45, MinimumLength = 2, ErrorMessage = "Cidade deve ter entre 2 e 45 caracteres")]
        public string Cidade { get; set; } = null!;

        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string Estado { get; set; } = null!;

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public string Tipo { get; set; } = null!;

        [Required(ErrorMessage = "Número de quartos é obrigatório")]
        [Range(0, 20, ErrorMessage = "Número de quartos deve ser entre 0 e 20")]
        public int Quartos { get; set; }

        [Required(ErrorMessage = "Número de banheiros é obrigatório")]
        [Range(1, 20, ErrorMessage = "Número de banheiros deve ser entre 1 e 20")]
        public int Banheiros { get; set; }

        [Required(ErrorMessage = "Área é obrigatória")]
        [Range(1.0, 9999.99, ErrorMessage = "Área deve ser entre 1 e 9999,99 m²")]
        public decimal Area { get; set; }

        [Required(ErrorMessage = "Vagas de garagem é obrigatório")]
        [Range(0, 20, ErrorMessage = "Vagas de garagem deve ser entre 0 e 20")]
        public int VagasGaragem { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(1.0, 999999.99, ErrorMessage = "Valor deve ser entre 1 e 999999,99")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 200 caracteres")]
        public string Descricao { get; set; } = null!;
    }
}