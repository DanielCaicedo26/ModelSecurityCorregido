﻿namespace Entity.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DocumentNumber { get; set; } = null!; // Nuevo campo obligatorio
        public string? DocumentType { get; set; }           // Nuevo campo opcional
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<StateInfraction>? StateInfractions { get; set; }
        public virtual ICollection<PaymentUser>? PaymentUsers { get; set; }
    }
}