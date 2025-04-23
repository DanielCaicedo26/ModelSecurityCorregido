using Bussines;
using Data;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar clases de Rol
builder.Services.AddScoped<AccessLogData>();
builder.Services.AddScoped<BillData>();
builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<InformationInfractionData>();
builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuloFormData>();
builder.Services.AddScoped<PaymentAgreementData>();
builder.Services.AddScoped<PaymentHistoryData>();
builder.Services.AddScoped<PaymentUserData>();
builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PersonData>();
builder.Services.AddScoped<RoleData>();
builder.Services.AddScoped<RoleFormPermissionData>();
builder.Services.AddScoped<RoleUserData>();
builder.Services.AddScoped<StateInfractionData>();
builder.Services.AddScoped<TypeInfractionData>();
builder.Services.AddScoped<TypePaymentData>();
builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserNotificationData>();

// Registrar clases de Bussines
builder.Services.AddScoped<AccessLogBusiness>();
builder.Services.AddScoped<BillBusiness>();
builder.Services.AddScoped<FormBusiness>();
builder.Services.AddScoped<InformationInfractionBusiness>();
builder.Services.AddScoped<ModuleBusiness>();
builder.Services.AddScoped<ModuloFormBusiness>();
builder.Services.AddScoped<PaymentAgreementBusiness>();
builder.Services.AddScoped<PaymentHistoryBusiness>();
builder.Services.AddScoped<PaymentUserBusiness>();
builder.Services.AddScoped<PermissionBusiness>();
builder.Services.AddScoped<PersonBusiness>();
builder.Services.AddScoped<RoleBusiness>();
builder.Services.AddScoped<RoleFormPermissionBusiness>();
builder.Services.AddScoped<RoleUserBusiness>();
builder.Services.AddScoped<StateInfractionBusiness>();
builder.Services.AddScoped<TypeInfractionBusiness>();
builder.Services.AddScoped<TypePaymentBusiness>();
builder.Services.AddScoped<UserBusiness>();
builder.Services.AddScoped<UserNotificationBusiness>();

// Configuraci�n de CORS
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");

// Agregar CORS y combinar pol�ticas
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("AllowSpecificOrigins", politica =>
    {
        politica.WithOrigins(OrigenesPermitidos)
                .WithOrigins("http://127.0.0.1:5500")  // Puedes a�adir m�s or�genes si es necesario
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el pipeline de solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar la pol�tica de CORS combinada
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();
app.MapControllers();
app.Run();
