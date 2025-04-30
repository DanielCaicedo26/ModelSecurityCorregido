namespace Entity.Dto
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DocumentNumber { get; set; } = null!;  // Nuevo campo obligatorio
        public string? DocumentType { get; set; }            // Nuevo campo opcional
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }
}