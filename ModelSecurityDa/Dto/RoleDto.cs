namespace Entity.Dto
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}