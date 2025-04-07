namespace Entity.Dto
{
    public class AccessLogDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public bool Status { get; set; } 
    }
}