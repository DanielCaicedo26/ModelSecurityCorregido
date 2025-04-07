namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateCreation { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<RoleFormPermission>? RolFormPermissions { get; set; }
        public virtual ICollection<ModuloForm>? ModuloForms { get; set; }
    }
}