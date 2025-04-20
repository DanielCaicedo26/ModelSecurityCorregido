namespace Entity.Dto
{
    public class AccessLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public bool Status { get; set; }

        public bool IsActive { get; set; }

        public string? Details { get; set; }
    }
}