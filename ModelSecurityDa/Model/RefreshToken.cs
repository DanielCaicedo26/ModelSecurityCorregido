using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Model
{
    /// <summary>
    /// Representa un token de actualización (refresh token) para mantener la sesión de un usuario
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Identificador único del refresh token
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Token único generado para la renovación
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// ID del usuario al que pertenece este refresh token
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Indica si el token ha sido utilizado
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Indica si el token ha sido revocado
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Fecha y hora en que fue añadido
        /// </summary>
        public DateTime AddedDate { get; set; }

        /// <summary>
        /// Fecha y hora de expiración
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// JTI (JSON Token Identifier) del token de acceso correspondiente
        /// </summary>
        [Required]
        public string JwtId { get; set; }

        /// <summary>
        /// Usuario al que pertenece este refresh token (relación de navegación)
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        /// <summary>
        /// Verificar si el token está activo (no usado, no revocado y no expirado)
        /// </summary>
        /// <returns>True si el token está activo, false en caso contrario</returns>
        public bool IsActive => !IsUsed && !IsRevoked && DateTime.UtcNow <= ExpiryDate;
    }
}