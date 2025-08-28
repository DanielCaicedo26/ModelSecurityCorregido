using Bussines;
using Bussines.interfaces;
using Bussines.Services;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Entity.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web2.Services;
using Web2.Services.Web2.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para debug
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Agregar DbContext - CORREGIR: usar GetConnectionString en lugar de "name="
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ModelSecurityDa API",
        Version = "v1",
        Description = "API para gestión de seguridad y facturación"
    });

    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Registrar clases de Data
builder.Services.AddScoped<BillData>();
builder.Services.AddScoped<InformationInfractionData>();
builder.Services.AddScoped<ModuloFormData>();
builder.Services.AddScoped<PaymentAgreementData>();
builder.Services.AddScoped<PaymentHistoryData>();
builder.Services.AddScoped<PaymentUserData>();
builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<RoleFormPermissionData>();
builder.Services.AddScoped<RoleUserData>();
builder.Services.AddScoped<StateInfractionData>();
builder.Services.AddScoped<TypeInfractionData>();
builder.Services.AddScoped<TypePaymentData>();
builder.Services.AddScoped<UserNotificationData>();

// Registrar clases de Business
builder.Services.AddScoped<AccessLogBusiness>();
builder.Services.AddScoped<BillBusiness>();
builder.Services.AddScoped<IFormBusiness, FormBusiness>();
builder.Services.AddScoped<InformationInfractionBusiness>();
builder.Services.AddScoped<IModuleBusiness, ModuleBusiness>();
builder.Services.AddScoped<IModuloFormBusiness, ModuloFormBusiness>();
builder.Services.AddScoped<PaymentAgreementBusiness>();
builder.Services.AddScoped<PaymentHistoryBusiness>();
builder.Services.AddScoped<PaymentUserBusiness>();
builder.Services.AddScoped<IPersonBusiness, PersonBusiness>();
builder.Services.AddScoped<RoleFormPermissionBusiness>();
builder.Services.AddScoped<IRoleUserBusiness, RoleUserBusiness>();
builder.Services.AddScoped<StateInfractionBusiness>();
builder.Services.AddScoped<TypeInfractionBusiness>();
builder.Services.AddScoped<TypePaymentBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IRoleBusiness, RoleBusiness>();
builder.Services.AddScoped<UserNotificationBusiness>();
builder.Services.AddScoped<IPermissionBusiness, PermissionBusiness>();

// Registrar repositorios
builder.Services.AddScoped<IAccessLogRepository, AccessLogRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleUserRepository, RoleUserRepository>();
builder.Services.AddScoped<IModuloFormRepository, ModuloFormRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

// Configuración de CORS
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("AllowOrigin", politica =>
    {
        politica.WithOrigins("http://localhost:3000",
                            "http://localhost:5500",
                            "http://127.0.0.1:5500",
                            "http://127.0.0.1:5501")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });

    opciones.AddPolicy("AllowAll", politica =>
    {
        politica.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

// Configurar autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey no está configurada");
}

var key = Encoding.UTF8.GetBytes(secretKey); // Cambiar a UTF8

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Para desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrEmpty(jwtSettings["Issuer"]),
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = !string.IsNullOrEmpty(jwtSettings["Audience"]),
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Registrar el servicio JWT
builder.Services.AddScoped<JwtAuthService>();

var app = builder.Build();

// Log de inicio
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Iniciando aplicación ModelSecurityDa...");

// Verificar conexión a base de datos al inicio
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var canConnect = await context.Database.CanConnectAsync();
        logger.LogInformation($"Conexión a base de datos: {(canConnect ? "ÉXITO" : "FALLIDA")}");

        if (!canConnect)
        {
            logger.LogError("No se pudo conectar a la base de datos. Verificar cadena de conexión.");
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Error al verificar conexión a base de datos");
}

// Configurar el pipeline de solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModelSecurityDa API V1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
    });
    logger.LogInformation("Swagger habilitado en: /swagger");
}

// REMOVER UseHttpsRedirection en Docker para evitar problemas
// app.UseHttpsRedirection();

// Usar la política de CORS - debe estar antes de Authentication
app.UseCors("AllowOrigin");

// Añadir middleware de autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

// Endpoint de health check
app.MapGet("/", () => Results.Ok(new
{
    status = "OK",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    message = "ModelSecurityDa API is running"
}));

app.MapGet("/health", async (ApplicationDbContext context) =>
{
    try
    {
        var canConnect = await context.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = canConnect ? "Healthy" : "Unhealthy",
            database = canConnect ? "Connected" : "Disconnected",
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Health check failed: {ex.Message}");
    }
});

app.MapControllers();

logger.LogInformation("Aplicación configurada. Esperando requests...");

app.Run();