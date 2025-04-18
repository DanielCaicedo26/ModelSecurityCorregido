namespace Entity.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int PersonId { get; set; }
        public bool IsActive { get; set; } = true; // Nuevo campo para indicar si el usuario está activo
    }
}