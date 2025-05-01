using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto
{
    public class TokenValidationDto
    {
        /// <summary>
        /// Indica si el token es válido
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// ID del usuario del token
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Nombre de usuario del token
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Tiempo restante de validez en segundos
        /// </summary>
        public int? RemainingTimeInSeconds { get; set; }
    }
}
