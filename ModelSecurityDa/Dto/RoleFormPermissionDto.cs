namespace Entity.Dto
{
    public class RoleFormPermissionDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }
    }
}