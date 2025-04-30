namespace Entity.Dto
{
    public class StateInfractionDto
    {
        public int Id { get; set; }
        public int InfractionId { get; set; }
        public int PersonId { get; set; }
        public DateTime DateViolation { get; set; }
        public decimal FineValue { get; set; }
        public string State { get; set; } = null!;
        public string? DocumentNumber { get; set; }  // Nuevo campo
        public bool IsActive { get; set; }
    }
}
