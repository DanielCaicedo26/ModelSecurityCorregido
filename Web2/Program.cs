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

// Agregar DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar clases de Rol

builder.Services.AddScoped<BillData>();
builder.Services.AddScoped<InformationInfractionData>();
builder.Services.AddScoped<ModuloFormData>();
builder.Services.AddScoped<PaymentAgreementData>();
builder.Services.AddScoped<PaymentHistoryData>();
builder.Services.AddScoped<PaymentUserData>();
builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<RoleData>();
builder.Services.AddScoped<RoleFormPermissionData>();
builder.Services.AddScoped<RoleUserData>();
builder.Services.AddScoped<StateInfractionData>();
builder.Services.AddScoped<TypeInfractionData>();
builder.Services.AddScoped<TypePaymentData>();

builder.Services.AddScoped<UserNotificationData>();

// Registrar clases de Bussines

builder.Services.AddScoped<AccessLogBusiness>();
builder.Services.AddScoped<BillBusiness>();
builder.Services.AddScoped<IFormBusiness, FormBusiness>();
builder.Services.AddScoped<InformationInfractionBusiness>();
builder.Services.AddScoped<IModuleBusiness, ModuleBusiness>();
builder.Services.AddScoped<ModuloFormBusiness>();
builder.Services.AddScoped<PaymentAgreementBusiness>();
builder.Services.AddScoped<PaymentHistoryBusiness>();
builder.Services.AddScoped<PaymentUserBusiness>();
builder.Services.AddScoped<PermissionBusiness>();
builder.Services.AddScoped<IPersonBusiness, PersonBusiness>();
builder.Services.AddScoped<RoleFormPermissionBusiness>();
builder.Services.AddScoped<RoleUserBusiness>();
builder.Services.AddScoped<StateInfractionBusiness>();
builder.Services.AddScoped<TypeInfractionBusiness>();
builder.Services.AddScoped<TypePaymentBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IRoleBusiness, RoleBusiness>();


builder.Services.AddScoped<UserNotificationBusiness>();

// Registrar repositorios
builder.Services.AddScoped<IAccessLogRepository, AccessLogRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IUserRepository , UserRepository>();
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();



// Configuración de CORS
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");

// Agregar CORS con el nombre correcto para que coincida con el atributo del controlador
builder.Services.AddCors(opciones =>
{
    // Política principal que se usa en el controlador con [EnableCors("AllowOrigin")]
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

    // Política alternativa para uso global si es necesario
    opciones.AddPolicy("AllowAll", politica =>
    {
        politica.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

// Configurar autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
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

// Configurar el pipeline de solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar la política de CORS - debe estar antes de Authentication
app.UseCors("AllowOrigin");

// Añadir middleware de autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();