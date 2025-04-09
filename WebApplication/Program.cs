using Bussines;
using Data;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        // Registrar clases de Rol
        builder.Services.AddScoped<RoleData>();
        builder.Services.AddScoped<RoleBusiness>();


        // Agregar CORS
        var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");
        builder.Services.AddCors(opciones =>
        {
            opciones.AddDefaultPolicy(politica =>
            {
                politica.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
            });
        });

        // Agregar DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
            opciones.UseSqlServer("name=DefaultConnection"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}