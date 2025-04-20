namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateCreation { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }


        public virtual ICollection<RoleFormPermission>? RoleFormPermissions { get; set; }
        public virtual ICollection<ModuloForm>? ModuloForms { get; set; }
    }
}