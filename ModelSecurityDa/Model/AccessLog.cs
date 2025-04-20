namespace Entity.Model
{
    public class AccessLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public bool Status { get; set; } 
        public string? Details { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual User User { get; set; } = null!;
    }
}