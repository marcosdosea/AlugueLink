using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ImovelViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Locador � obrigat�rio")]
        public int IdLocador { get; set; }

        [Required(ErrorMessage = "CEP � obrigat�rio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "CEP deve ter 8 caracteres")]
        public string Cep { get; set; } = null!;

        [Required(ErrorMessage = "Logradouro � obrigat�rio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Logradouro deve ter entre 5 e 100 caracteres")]
        public string Logradouro { get; set; } = null!;

        [Required(ErrorMessage = "N�mero � obrigat�rio")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "N�mero deve ter entre 1 e 5 caracteres")]
        public string Numero { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Complemento deve ter no m�ximo 50 caracteres")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro � obrigat�rio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bairro deve ter entre 2 e 50 caracteres")]
        public string Bairro { get; set; } = null!;

        [Required(ErrorMessage = "Cidade � obrigat�ria")]
        [StringLength(45, MinimumLength = 2, ErrorMessage = "Cidade deve ter entre 2 e 45 caracteres")]
        public string Cidade { get; set; } = null!;

        [Required(ErrorMessage = "Estado � obrigat�rio")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres")]
        public string Estado { get; set; } = null!;

        [Required(ErrorMessage = "Tipo � obrigat�rio")]
        public string Tipo { get; set; } = null!;

        [Required(ErrorMessage = "N�mero de quartos � obrigat�rio")]
        [Range(0, 20, ErrorMessage = "N�mero de quartos deve ser entre 0 e 20")]
        public int Quartos { get; set; }

        [Required(ErrorMessage = "N�mero de banheiros � obrigat�rio")]
        [Range(1, 20, ErrorMessage = "N�mero de banheiros deve ser entre 1 e 20")]
        public int Banheiros { get; set; }

        [Required(ErrorMessage = "�rea � obrigat�ria")]
        [Range(1.0, 9999.99, ErrorMessage = "�rea deve ser entre 1 e 9999,99 m�")]
        public decimal Area { get; set; }

        [Required(ErrorMessage = "Vagas de garagem � obrigat�rio")]
        [Range(0, 20, ErrorMessage = "Vagas de garagem deve ser entre 0 e 20")]
        public int VagasGaragem { get; set; }

        [Required(ErrorMessage = "Valor � obrigat�rio")]
        [Range(1.0, 999999.99, ErrorMessage = "Valor deve ser entre 1 e 999999,99")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Descri��o � obrigat�ria")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Descri��o deve ter entre 10 e 200 caracteres")]
        public string Descricao { get; set; } = null!;
    }
}