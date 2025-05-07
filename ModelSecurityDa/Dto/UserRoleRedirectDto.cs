namespace Entity.Dto
{
    /// <summary>
    /// DTO para manejar la redirección después del inicio de sesión basado en si el usuario es administrador o no
    /// </summary>
    public class UserRoleRedirectDto
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// Indica si el usuario es administrador
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Ruta de redirección después del inicio de sesión
        /// </summary>
        public string RedirectUrl { get; set; } = null!;
    }
}