using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto
{
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT generado
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Fecha y hora de expiración del token
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Información básica del usuario autenticado
        /// </summary>
        public UserInfoDto User { get; set; }
        public string RefreshToken { get; set; }
        public UserRoleRedirectDto RoleRedirection { get; set; }
    }
}
