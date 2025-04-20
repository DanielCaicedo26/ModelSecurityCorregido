

namespace Entity.Dto
{

    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public bool IsActive { get; set; }

    }
}

