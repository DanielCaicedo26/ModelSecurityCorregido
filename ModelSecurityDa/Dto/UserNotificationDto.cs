namespace Entity.Dto
{
    public class UserNotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public bool IsActive { get; set; } // Nuevo campo
    }
}