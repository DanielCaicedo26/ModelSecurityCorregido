namespace Entity.Dto
{
    public class StateInfractionDto
    {
        public int Id { get; set; }
        public DateTime DateViolation { get; set; }
        public decimal FineValue { get; set; }
        public string State { get; set; } = null!;
    }
}