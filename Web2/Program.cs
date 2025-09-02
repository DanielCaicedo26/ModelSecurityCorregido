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
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Web2.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para debug
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// --- NEW: Register all three DbContexts ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddDbContext<ApplicationDbContextPostgres>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"), 
        b => b.MigrationsAssembly("Web2")));

builder.Services.AddDbContext<ApplicationDbContextMySql>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySql"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")),
        b => b.MigrationsAssembly("Web2")));

// --- NEW: Register services for per-request DB switching ---
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IDbContextProvider, PerRequestDbContextProvider>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- NEW: Update Swagger to include custom header ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ModelSecurityDa API",
        Version = "v1",
        Description = "API para gestión de seguridad y facturación. Use the 'X-Database-Engine' header to choose 'sqlserver', 'postgres', or 'mysql'."
    });

    // Add custom header for database selection
    c.OperationFilter<AddRequiredHeaderParameter>();


    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
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

var key = Encoding.UTF8.GetBytes(secretKey);

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

builder.Services.AddScoped<JwtAuthService>();

var app = builder.Build();

// Log de inicio
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Iniciando aplicación ModelSecurityDa...");

// --- NEW: Migration logic for all three databases ---
try
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var mainLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // Migrate SQL Server
        try
        {
            mainLogger.LogInformation("Applying SQL Server migrations...");
            var sqlContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await sqlContext.Database.MigrateAsync();
            mainLogger.LogInformation("SQL Server migrations applied successfully.");
        }
        catch (Exception)
        {
            mainLogger.LogError("Error applying SQL Server migrations.");
        }

        // Migrate PostgreSQL
        try
        {
            mainLogger.LogInformation("Applying PostgreSQL migrations...");
            var postgresContext = serviceProvider.GetRequiredService<ApplicationDbContextPostgres>();
            await postgresContext.Database.MigrateAsync();
            mainLogger.LogInformation("PostgreSQL migrations applied successfully.");
        }
        catch (Exception)
        {
            mainLogger.LogInformation("Error applying PostgreSQL migrations.");
        }

        // Migrate MySQL
        try
        {
            mainLogger.LogInformation("Applying MySQL migrations...");
            var mysqlContext = serviceProvider.GetRequiredService<ApplicationDbContextMySql>();
            await mysqlContext.Database.MigrateAsync();
            mainLogger.LogInformation("MySQL migrations applied successfully.");
        }
        catch (Exception ex)
        {
            mainLogger.LogError(ex, "Error applying MySQL migrations.");
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during the initial database migration setup.");
}


// Configurar el pipeline de solicitudes - Swagger habilitado también en Production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModelSecurityDa API V1");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});
logger.LogInformation("Swagger habilitado en: /swagger");

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

// --- NEW: Updated Health Check ---
app.MapGet("/health", async (IDbContextProvider dbProvider) =>
{
    try
    {
        var context = dbProvider.GetDbContext();
        var canConnect = await context.Database.CanConnectAsync();
        var dbName = context.Database.ProviderName;

        return Results.Ok(new
        {
            status = canConnect ? "Healthy" : "Unhealthy",
            database = canConnect ? "Connected" : "Disconnected",
            engine = dbName,
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

// --- NEW: Swagger Operation Filter Class ---
public class AddRequiredHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Database-Engine",
            In = ParameterLocation.Header,
            Description = "Database engine to use (sqlserver, postgres, mysql)",
            Required = false, // Set to false so it's not mandatory for all requests
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new Microsoft.OpenApi.Any.OpenApiString("sqlserver")
            }
        });
    }
}