// TypeInfraction
namespace Entity.Model
{
    public class TypeInfraction
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Nueva propiedad añadida
        public string TypeViolation { get; set; } = null!;
        public decimal ValueInfraction { get; set; }
        public string? Description { get; set; }
        public string? InformationFine { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<StateInfraction>? StateInfraction { get; set; }
        public virtual ICollection<InformationInfraction>? InformationInfraction { get; set; }
    }
}