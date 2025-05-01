using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto
{
    public class RegisterUserDto
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        public string Username { get; set; }

        /// <summary>
        /// Correo electrónico
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        public string Email { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de contraseña
        /// </summary>
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Nombre
        /// </summary>
        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; }

        /// <summary>
        /// Apellido
        /// </summary>
        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Número de documento
        /// </summary>
        [Required(ErrorMessage = "El número de documento es requerido")]
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Teléfono (opcional)
        /// </summary>
        public string Phone { get; set; }
    }

}
