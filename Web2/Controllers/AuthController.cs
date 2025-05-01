using Bussines;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Web2.Services;
using Web2.Services.Web2.Services;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly JwtAuthService _jwtAuthService;
        private readonly AccessLogBusiness _accessLogBusiness;

        public AuthController(
            ILogger<AuthController> logger,
            ApplicationDbContext context,
            JwtAuthService jwtAuthService,
            AccessLogBusiness accessLogBusiness)
        {
            _logger = logger;
            _context = context;
            _jwtAuthService = jwtAuthService;
            _accessLogBusiness = accessLogBusiness;
        }

        /// <summary>
        /// Iniciar sesión y obtener token JWT
        /// </summary>
        /// <param name="model">Credenciales de inicio de sesión</param>
        /// <returns>Token JWT y datos del usuario</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest(new { message = "Usuario y contraseña son requeridos" });
                }

                // Buscar el usuario por nombre de usuario
                var user = await _context.User
                    .Include(u => u.Person)
                    .Include(u => u.RoleUsers)
                        .ThenInclude(ru => ru.Role)
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive);

                if (user == null)
                {
                    // Registro de intento fallido
                    await LogLoginAttempt(0, model.Username, false, "Usuario no encontrado");
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
                }

                // Hashear la contraseña para comparar
                var hashedPassword = HashPassword(model.Password);

                // Verificar la contraseña
                if (user.Password != hashedPassword)
                {
                    // Registro de intento fallido
                    await LogLoginAttempt(user.Id, user.Username, false, "Contraseña incorrecta");
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
                }

                // Generar token JWT y refresh token
                var authResponse = await _jwtAuthService.GenerateJwtToken(user);

                // Registro de inicio de sesión exitoso
                await LogLoginAttempt(user.Id, user.Username, true, "Login exitoso");

                // Devolver token y datos del usuario
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en inicio de sesión");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        /// <param name="model">Datos de registro</param>
        /// <returns>Resultado del registro</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Datos de registro inválidos" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validar datos del modelo
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password) ||
                    string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.FirstName) ||
                    string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.DocumentNumber))
                {
                    return BadRequest(new { message = "Todos los campos obligatorios deben ser completados" });
                }

                // Verificar si el usuario ya existe
                if (await _context.User.AnyAsync(u => u.Username == model.Username))
                {
                    return BadRequest(new { message = "El nombre de usuario ya está en uso" });
                }

                // Verificar si el correo ya está registrado
                if (await _context.User.AnyAsync(u => u.Email == model.Email))
                {
                    return BadRequest(new { message = "El correo electrónico ya está registrado" });
                }

                // Verificar si el documento ya está registrado
                if (await _context.Person.AnyAsync(p => p.DocumentNumber == model.DocumentNumber))
                {
                    return BadRequest(new { message = "El número de documento ya está registrado" });
                }

                // Crear la persona
                var person = new Person
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DocumentNumber = model.DocumentNumber,
                    DocumentType = model.DocumentType ?? "CC",
                    Phone = model.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Person.AddAsync(person);
                await _context.SaveChangesAsync();

                // Crear el usuario
                var hashedPassword = HashPassword(model.Password);
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = hashedPassword,
                    PersonId = person.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();

                // Asignar rol básico si existe (opcional)
                var basicRole = await _context.Role.FirstOrDefaultAsync(r => r.RoleName == "Usuario");
                if (basicRole != null)
                {
                    var roleUser = new RoleUser
                    {
                        RoleId = basicRole.Id,
                        UserId = user.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Set<RoleUser>().AddAsync(roleUser);
                    await _context.SaveChangesAsync();
                }

                // Registrar evento de registro
                await LogEvent(user.Id, "Registro de usuario", true, $"Nuevo usuario registrado: {model.Username}");

                await transaction.CommitAsync();

                // Opcionalmente, realizar inicio de sesión automático
                var authResponse = await _jwtAuthService.GenerateJwtToken(user);
                return Ok(new
                {
                    message = "Usuario registrado correctamente",
                    token = authResponse.Token,
                    refreshToken = authResponse.RefreshToken,
                    user = authResponse.User
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error en registro de usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Validar un token JWT
        /// </summary>
        /// <returns>Estado de validez del token</returns>
        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            try
            {
                // Si llegamos aquí, el token es válido (Authorize lo validó)
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;

                return Ok(new TokenValidationDto
                {
                    IsValid = true,
                    UserId = int.Parse(userId),
                    Username = username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar token");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verifica la validez de un token sin requerir autorización
        /// </summary>
        /// <param name="token">Token a verificar</param>
        /// <returns>Información sobre la validez del token</returns>
        [HttpPost("check-token")]
        [AllowAnonymous]
        public IActionResult CheckToken([FromBody] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Token no proporcionado" });
                }

                var validationResult = _jwtAuthService.ValidateToken(token);
                return Ok(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar token");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un token JWT expirado usando un refresh token
        /// </summary>
        /// <param name="model">Token y refresh token</param>
        /// <returns>Nuevo token JWT y refresh token</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.RefreshToken))
                {
                    return BadRequest(new { message = "Token y refresh token son requeridos" });
                }

                var authResponse = await _jwtAuthService.VerifyAndGenerateNewToken(model);
                if (authResponse == null)
                {
                    return Unauthorized(new { message = "Token o refresh token inválido" });
                }

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al refrescar token");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cerrar sesión
        /// </summary>
        /// <returns>Resultado de cierre de sesión</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Obtener ID de usuario desde el token
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;

                if (int.TryParse(userIdStr, out int userId))
                {
                    // Revocar todos los refresh tokens del usuario
                    await _jwtAuthService.RevokeAllRefreshTokens(userId);

                    // Registrar logout
                    await LogEvent(userId, "Logout", true, $"Cierre de sesión para usuario: {username}");
                }

                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en cierre de sesión");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cambiar la contraseña de un usuario
        /// </summary>
        /// <param name="model">Datos para cambio de contraseña</param>
        /// <returns>Resultado del cambio de contraseña</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.CurrentPassword) ||
                    string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmNewPassword))
                {
                    return BadRequest(new { message = "Todos los campos son requeridos" });
                }

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    return BadRequest(new { message = "Las contraseñas nuevas no coinciden" });
                }

                // Obtener ID de usuario desde el token
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return BadRequest(new { message = "ID de usuario inválido" });
                }

                // Buscar el usuario
                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Verificar contraseña actual
                var hashedCurrentPassword = HashPassword(model.CurrentPassword);
                if (user.Password != hashedCurrentPassword)
                {
                    return BadRequest(new { message = "Contraseña actual incorrecta" });
                }

                // Actualizar contraseña
                user.Password = HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();

                // Revocar todos los refresh tokens existentes
                await _jwtAuthService.RevokeAllRefreshTokens(userId);

                // Registrar cambio de contraseña
                await LogEvent(userId, "Cambio de contraseña", true, "Contraseña actualizada correctamente");

                return Ok(new { message = "Contraseña actualizada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        #region Métodos privados

        /// <summary>
        /// Hashear una contraseña usando SHA256
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña</returns>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Registra un intento de inicio de sesión
        /// </summary>
        private async Task LogLoginAttempt(int userId, string username, bool success, string details)
        {
            try
            {
                await _accessLogBusiness.CreateAccessLogAsync(
                    new AccessLogDto
                    {
                        Action = success ? "Login exitoso" : "Login fallido",
                        Status = success,
                        Details = $"{details} - Usuario: {username}",
                        IsActive = true
                    },
                    userId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar intento de login");
            }
        }

        /// <summary>
        /// Registra un evento en el log de acceso
        /// </summary>
        private async Task LogEvent(int userId, string action, bool status, string details)
        {
            try
            {
                await _accessLogBusiness.CreateAccessLogAsync(
                    new AccessLogDto
                    {
                        Action = action,
                        Status = status,
                        Details = details,
                        IsActive = true
                    },
                    userId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar evento");
            }
        }

        #endregion
    }
}