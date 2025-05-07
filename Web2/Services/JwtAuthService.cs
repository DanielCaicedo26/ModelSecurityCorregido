namespace Web2.Services
{
    using Entity.Context;
    using Entity.Dto;
    using Entity.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    namespace Web2.Services
    {
        /// <summary>
        /// Servicio para gestionar la autenticación con JWT y refresh tokens
        /// </summary>
        public class JwtAuthService
        {
            private readonly IConfiguration _configuration;
            private readonly ApplicationDbContext _context;

            /// <summary>
            /// Constructor del servicio de autenticación JWT
            /// </summary>
            /// <param name="configuration">Configuración de la aplicación</param>
            /// <param name="context">Contexto de la base de datos</param>
            public JwtAuthService(IConfiguration configuration, ApplicationDbContext context)
            {
                _configuration = configuration;
                _context = context;
            }

            /// <summary>
            /// Genera un nuevo token JWT para el usuario especificado
            /// </summary>
            /// <param name="user">Usuario para el que se genera el token</param>
            /// <returns>Token JWT y datos relacionados</returns>
            public async Task<AuthResponseDto> GenerateJwtToken(User user)
            {
                // Obtener configuración JWT
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var issuer = jwtSettings["Issuer"] ?? "securityauthapi";
                var audience = jwtSettings["Audience"] ?? "securityauthclient";
                var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");

                // Crear un identificador único para este token
                var jti = Guid.NewGuid().ToString();

                // Crear claims para el token
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, jti),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                // Agregar claims para los roles del usuario
                if (user.RoleUsers != null && user.RoleUsers.Any())
                {
                    // Cargar roles si no están cargados
                    if (!user.RoleUsers.Any(ru => ru.Role != null))
                    {
                        await _context.Entry(user)
                            .Collection(u => u.RoleUsers)
                            .Query()
                            .Include(ru => ru.Role)
                            .LoadAsync();
                    }

                    var roles = user.RoleUsers
                        .Where(ru => ru.IsActive && ru.Role != null)
                        .Select(ru => ru.Role.RoleName)
                        .ToArray();

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Crear clave de firma y credenciales
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Calcular fecha de expiración
                var expiration = DateTime.UtcNow.AddMinutes(expirationInMinutes);

                // Crear token JWT
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds
                );

                // Generar refresh token
                var refreshToken = await GenerateRefreshToken(user.Id, jti, int.Parse(jwtSettings["RefreshTokenExpirationInDays"] ?? "7"));

                // Obtener roles para el DTO de respuesta
                var userRoles = user.RoleUsers?
                    .Where(ru => ru.IsActive && ru.Role != null)
                    .Select(ru => ru.Role.RoleName)
                    .ToArray() ?? Array.Empty<string>();

                // Verificar si el usuario es administrador
                bool isAdmin = userRoles.Any(r => r.Equals("Administrador", StringComparison.OrdinalIgnoreCase));

                // Crear el DTO de redirección
                var roleRedirect = new UserRoleRedirectDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    IsAdmin = isAdmin,
                    RedirectUrl = isAdmin ? "http://127.0.0.1:5501/Administrador/html/person.html" : "http://127.0.0.1:5501/Administrador/html/rolUser.html"  // Ajusta estas rutas según tu proyecto
                };

                // Crear respuesta con token y datos de usuario
                return new AuthResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken.Token,
                    Expiration = expiration,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.Person?.FirstName,
                        LastName = user.Person?.LastName,
                        Roles = userRoles
                    },
                    RoleRedirection = roleRedirect  // Agregar la información de redirección
                };
            }

            /// <summary>
            /// Genera un nuevo refresh token para el usuario
            /// </summary>
            /// <param name="userId">ID del usuario</param>
            /// <param name="jwtId">ID del token JWT asociado</param>
            /// <param name="expirationInDays">Días hasta la expiración</param>
            /// <returns>El objeto RefreshToken creado</returns>
            private async Task<RefreshToken> GenerateRefreshToken(int userId, string jwtId, int expirationInDays)
            {
                // Generar un token aleatorio
                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                var token = Convert.ToBase64String(randomBytes);

                // Crear el objeto refresh token
                var refreshToken = new RefreshToken
                {
                    Token = token,
                    UserId = userId,
                    JwtId = jwtId,
                    IsUsed = false,
                    IsRevoked = false,
                    AddedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(expirationInDays)
                };

                // Guardar en la base de datos
                await _context.Set<RefreshToken>().AddAsync(refreshToken);
                await _context.SaveChangesAsync();

                return refreshToken;
            }

            /// <summary>
            /// Verifica y actualiza un refresh token
            /// </summary>
            /// <param name="tokenDto">DTO con el token y refresh token</param>
            /// <returns>Nueva respuesta de autenticación o null si el refresh token es inválido</returns>
            public async Task<AuthResponseDto> VerifyAndGenerateNewToken(RefreshTokenDto tokenDto)
            {
                // Validar el token JWT para obtener el JTI y userId (aunque esté expirado)
                var tokenHandler = new JwtSecurityTokenHandler();

                // Primero verificamos si el formato del token es válido
                if (!tokenHandler.CanReadToken(tokenDto.Token))
                {
                    return null;
                }

                var jwtToken = tokenHandler.ReadJwtToken(tokenDto.Token);
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return null;
                }

                // Buscar el refresh token en la base de datos
                var storedRefreshToken = await _context.Set<RefreshToken>()
                    .FirstOrDefaultAsync(rt => rt.Token == tokenDto.RefreshToken);

                // Verificar que el refresh token exista y sea válido
                if (storedRefreshToken == null ||
                    !storedRefreshToken.IsActive ||
                    storedRefreshToken.JwtId != jti ||
                    storedRefreshToken.UserId != userId)
                {
                    return null;
                }

                // Marcar el refresh token como usado
                storedRefreshToken.IsUsed = true;
                _context.Set<RefreshToken>().Update(storedRefreshToken);
                await _context.SaveChangesAsync();

                // Buscar el usuario
                var user = await _context.User
                    .Include(u => u.Person)
                    .Include(u => u.RoleUsers)
                        .ThenInclude(ru => ru.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return null;
                }

                // Generar un nuevo token JWT y refresh token
                return await GenerateJwtToken(user);
            }

            /// <summary>
            /// Revoca todos los refresh tokens de un usuario
            /// </summary>
            /// <param name="userId">ID del usuario</param>
            /// <returns>True si se revocaron los tokens, false en caso contrario</returns>
            public async Task<bool> RevokeAllRefreshTokens(int userId)
            {
                try
                {
                    var refreshTokens = await _context.Set<RefreshToken>()
                        .Where(rt => rt.UserId == userId && !rt.IsUsed && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow)
                        .ToListAsync();

                    if (!refreshTokens.Any())
                    {
                        return true;
                    }

                    foreach (var token in refreshTokens)
                    {
                        token.IsRevoked = true;
                    }

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// Valida un token JWT
            /// </summary>
            /// <param name="token">Token JWT a validar</param>
            /// <returns>Información sobre la validez del token</returns>
            public TokenValidationDto ValidateToken(string token)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return new TokenValidationDto { IsValid = false };
                }

                try
                {
                    // Obtener configuración JWT
                    var jwtSettings = _configuration.GetSection("JwtSettings");
                    var secretKey = jwtSettings["SecretKey"];
                    var issuer = jwtSettings["Issuer"] ?? "securityauthapi";
                    var audience = jwtSettings["Audience"] ?? "securityauthclient";

                    // Crear clave de validación
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                    // Configurar parámetros de validación
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = !string.IsNullOrEmpty(issuer),
                        ValidIssuer = issuer,
                        ValidateAudience = !string.IsNullOrEmpty(audience),
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    // Validar el token
                    tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                    var jwtToken = (JwtSecurityToken)validatedToken;

                    // Obtener información del token
                    var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub || x.Type == ClaimTypes.NameIdentifier).Value);
                    var username = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName || x.Type == ClaimTypes.Name).Value;
                    var expiration = jwtToken.ValidTo;
                    var remainingTime = (int)(expiration - DateTime.UtcNow).TotalSeconds;

                    return new TokenValidationDto
                    {
                        IsValid = true,
                        UserId = userId,
                        Username = username,
                        RemainingTimeInSeconds = remainingTime
                    };
                }
                catch (SecurityTokenExpiredException)
                {
                    // El token ha expirado pero es válido en otros aspectos
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);

                        var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub || x.Type == ClaimTypes.NameIdentifier).Value);
                        var username = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName || x.Type == ClaimTypes.Name).Value;

                        return new TokenValidationDto
                        {
                            IsValid = false,
                            UserId = userId,
                            Username = username,
                            RemainingTimeInSeconds = 0
                        };
                    }
                    catch
                    {
                        return new TokenValidationDto { IsValid = false };
                    }
                }
                catch
                {
                    // Cualquier otra excepción significa token inválido
                    return new TokenValidationDto { IsValid = false };
                }
            }
        }
    }
}