// Role
namespace Entity.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<RoleUser>? RoleUsers { get; set; }
        public virtual ICollection<RoleFormPermission>? RoleFormPermissions { get; set; }
    }
}