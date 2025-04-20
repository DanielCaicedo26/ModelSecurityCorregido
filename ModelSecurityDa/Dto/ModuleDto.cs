namespace Entity.Dto
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Statu { get; set; }

        public bool IsActive { get; set; }
    }
}