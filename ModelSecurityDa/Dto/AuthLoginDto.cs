using System;
using System.ComponentModel.DataAnnotations;

namespace Entity.Dto
{
    /// <summary>
    /// DTO para solicitudes de inicio de sesión
    /// </summary>
    public class AuthLoginDto
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
    }
}