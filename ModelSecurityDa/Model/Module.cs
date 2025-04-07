// Module
namespace Entity.Model
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Statu { get; set; }
        public virtual ICollection<ModuloForm>? ModuloForms { get; set; }
    }
}