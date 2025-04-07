namespace Entity.Dto
{
    public class TypeInfractionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TypeViolation { get; set; } = null!;
        public decimal ValueInfraction { get; set; }
    }
}