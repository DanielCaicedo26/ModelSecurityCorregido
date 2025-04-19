namespace Entity.Model
{
    public class UserNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public bool IsActive { get; set; } = true;// Nuevo campo
        public DateTime CreatedAt { get; set; }
        public virtual User User { get; set; } = null!;
    }
}