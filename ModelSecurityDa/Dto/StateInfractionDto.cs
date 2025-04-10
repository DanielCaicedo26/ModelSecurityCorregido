namespace Entity.Dto
{
    public class StateInfractionDto
    {
        public int InfractionId;
        public int PersonId;

        public int Id { get; set; }
        public DateTime DateViolation { get; set; }
        public decimal FineValue { get; set; }
        public string State { get; set; } = null!;
    }
}