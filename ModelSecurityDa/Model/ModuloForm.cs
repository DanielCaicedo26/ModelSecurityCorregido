namespace Entity.Model
{
    public class ModuloForm
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public int ModuleId { get; set; }

        public bool IsActive { get; set; }
        public virtual Form? Form { get; set; }
        public virtual Module? Module { get; set; }
    }
}