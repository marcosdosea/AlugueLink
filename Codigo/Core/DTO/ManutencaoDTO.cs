namespace Core.DTO
{
    public class ManutencaoDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = null!;
        public DateTime DataSolicitacao { get; set; }
        public string Status { get; set; } = null!;
        public decimal Valor { get; set; }
        public int Idimovel { get; set; }
    }
}