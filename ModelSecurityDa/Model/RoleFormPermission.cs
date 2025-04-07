namespace Entity.Model
{
    public class RoleFormPermission
    {
        public int Id { get; set; }

        public int RoleId { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public virtual Role? Role { get; set; }
        public virtual Form? Form { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}