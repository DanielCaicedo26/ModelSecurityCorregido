using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto
{
    public class RefreshTokenDto
    {
        /// <summary>
        /// Token JWT expirado o cerca de expirar
        /// </summary>
        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; }
    }
}
