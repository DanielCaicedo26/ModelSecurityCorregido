namespace Entity.Model
{
    public class Person

    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propiedades de navegación
        public User User { get; set; } 
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<StateInfraction>? StateInfractions { get; set; }
        public virtual ICollection<PaymentUser>? PaymentUsers { get; set; }
    }
}