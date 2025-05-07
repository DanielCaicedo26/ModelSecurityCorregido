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

        /// <summary>
        /// Token de actualización
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Información de redirección basada en el rol del usuario
        /// </summary>
        public UserRoleRedirectDto RoleRedirection { get; set; }
    }
}