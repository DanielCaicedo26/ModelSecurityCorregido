
namespace Entity.Model
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<RoleFormPermission>? RoleFormPermissions { get; set; }
    }
}