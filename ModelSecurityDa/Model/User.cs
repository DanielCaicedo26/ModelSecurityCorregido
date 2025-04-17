namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!; // Nueva propiedad añadida
        public int PersonId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Person Person { get; set; } = null!;
        
        public virtual ICollection<RoleUser>? RoleUsers { get; set; }
        public virtual ICollection<UserNotification>? UserNotifications { get; set; }
        public virtual ICollection<PaymentHistory>? PaymentHistories { get; set; }
        public ICollection<AccessLog> AccessLog { get; set; }

        public virtual ICollection<TypeInfraction>? TypeInfractions { get; set; }
    }
}