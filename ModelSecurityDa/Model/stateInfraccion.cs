﻿namespace Entity.Model
{
    public class StateInfraction
    {
        public int Id { get; set; }
        public int InfractionId { get; set; }
        public int PersonId { get; set; }
        public DateTime DateViolation { get; set; }
        public decimal FineValue { get; set; }
        public string State { get; set; } = null!;
        public string? DocumentNumber { get; set; }  // Nuevo campo
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Person Person { get; set; } = null!;
        public virtual TypeInfraction Infraction { get; set; } = null!;
    }
}