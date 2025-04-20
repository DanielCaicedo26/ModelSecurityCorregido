namespace Entity.Dto
{
    public class FormDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateCreation { get; set; }
        public bool IsActive { get; set; }
        public string? Status { get; set; }
    }
}