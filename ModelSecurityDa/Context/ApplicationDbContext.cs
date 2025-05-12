using Dapper;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace Entity.Context
{
    /// <summary>
    /// Representa el contexto de la base de datos de la aplicación, proporcionando configuraciones y métodos
    /// para la gestión de entidades y consultas personalizadas con Dapper.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del contexto de la base de datos.
        /// </summary>
        /// <param name="options">Opciones de configuración para el contexto de base de datos.</param>
        /// <param name="configuration">Instancia de IConfiguration para acceder a la configuración de la aplicación.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<AccessLog> AccessLog { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<InformationInfraction> InformationInfraction { get; set; }
        public DbSet<Entity.Model.Module> Module { get; set; }
        public DbSet<ModuloForm> ModuloForm { get; set; }
        public DbSet<PaymentAgreement> PaymentAgreement { get; set; }
        public DbSet<PaymentHistory> PaymentHistory { get; set; }
        public DbSet<PaymentUser> PaymentUser { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleFormPermission> RoleFormPermission { get; set; }
        public DbSet<RoleUser> RoleUser { get; set; }
        public DbSet<StateInfraction> StateInfraction { get; set; }
        public DbSet<TypeInfraction> TypeInfraction { get; set; }
        public DbSet<TypePayment> TypePayment { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }





        /// <summary>
        /// Configura los modelos de la base de datos aplicando configuraciones desde ensamblados.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo de base de datos.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Bill>()
                   .HasOne(b => b.PaymentAgreement)
                   .WithMany(pa => pa.Bills)
                   .HasForeignKey(b => b.PaymentAgreementId);

            modelBuilder.Entity<RefreshToken>()
           .HasOne(rt => rt.User)
           .WithMany()
           .HasForeignKey(rt => rt.UserId);


            modelBuilder.Entity<User>()
                   .HasOne(u => u.Person)
                   .WithOne(p => p.User)
                   .HasForeignKey<User>(u => u.PersonId);


            modelBuilder.Entity<Person>()
             .HasOne(p => p.User)
             .WithOne(u => u.Person)
             .HasForeignKey<User>(u => u.PersonId);

            modelBuilder.Entity<AccessLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.AccessLogs)
                .HasForeignKey(al => al.UserId);

            modelBuilder.Entity<RoleUser>()
                .HasOne(ru => ru.User)
                .WithMany(u => u.RoleUsers)
                .HasForeignKey(ru => ru.UserId);

            modelBuilder.Entity<RoleUser>()
                .HasOne(ru => ru.Role)
                .WithMany(r => r.RoleUsers)
                .HasForeignKey(ru => ru.RoleId);

            modelBuilder.Entity<ModuloForm>()
                .HasOne(mf => mf.Form)
                .WithMany(f => f.ModuloForms)
                .HasForeignKey(mf => mf.FormId);

            modelBuilder.Entity<ModuloForm>()
                .HasOne(mf => mf.Module)
                .WithMany(m => m.ModuloForms)
                .HasForeignKey(mf => mf.ModuleId);

            modelBuilder.Entity<RoleFormPermission>()
                .HasOne(rfp => rfp.Role)
                .WithMany(r => r.RoleFormPermissions)
                .HasForeignKey(rfp => rfp.RoleId);

            modelBuilder.Entity<RoleFormPermission>()
                .HasOne(rfp => rfp.Form)
                .WithMany(f => f.RoleFormPermissions)
                .HasForeignKey(rfp => rfp.FormId);

            modelBuilder.Entity<RoleFormPermission>()
                .HasOne(rfp => rfp.Permission)
                .WithMany(p => p.RoleFormPermissions)
                .HasForeignKey(rfp => rfp.PermissionId);

            modelBuilder.Entity<TypeInfraction>()
                .HasOne(ti => ti.User)
                .WithMany(u => u.TypeInfractions)
                .HasForeignKey(ti => ti.UserId);

            modelBuilder.Entity<StateInfraction>()
                .HasOne(si => si.Infraction)
                .WithMany(ti => ti.StateInfraction)
                .HasForeignKey(si => si.InfractionId);

            modelBuilder.Entity<StateInfraction>()
                .HasOne(si => si.Person)
                .WithMany(p => p.StateInfractions)
                .HasForeignKey(si => si.PersonId);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.PaymentAgreement)
                .WithMany(pa => pa.Bills)
                .HasForeignKey(b => b.PaymentAgreementId);

            modelBuilder.Entity<PaymentHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PaymentHistories)
                .HasForeignKey(ph => ph.UserId);

            modelBuilder.Entity<PaymentUser>()
                .HasOne(pu => pu.Person)
                .WithMany(p => p.PaymentUsers)
                .HasForeignKey(pu => pu.PersonId);

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<PaymentHistory>()
            .HasOne(ph => ph.InformationInfraction)
            .WithMany(ii => ii.PaymentHistory)
            .HasForeignKey(ph => ph.InformationInfractionId);

        }



        /// <summary>
        /// Configura opciones adicionales del contexto, como el registro de datos sensibles.
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones de configuración del contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            // Otras configuraciones adicionales pueden ir aquí
        }

        /// <summary>
        /// Configura convenciones de tipos de datos, estableciendo la precisión por defecto de los valores decimales.
        /// </summary>
        /// <param name="configurationBuilder">Constructor de configuración de modelos.</param>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos, asegurando la auditoría antes de persistir los datos.
        /// </summary>
        /// <returns>Número de filas afectadas.</returns>
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Guarda los cambios en la base de datos de manera asíncrona, asegurando la auditoría antes de la persistencia.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indica si se deben aceptar todos los cambios en caso de éxito.</param>
        /// <param name="cancellationToken">Token de cancelación para abortar la operación.</param>
        /// <returns>Número de filas afectadas de forma asíncrona.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve una colección de resultados de tipo genérico.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Una colección de objetos del tipo especificado.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object? parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        /// <summary>
        /// Ejecuta una consulta SQL utilizando Dapper y devuelve un solo resultado o el valor predeterminado si no hay resultados.
        /// </summary>
        /// <typeparam name="T">Tipo de los datos de retorno.</typeparam>
        /// <param name="text">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros opcionales de la consulta.</param>
        /// <param name="timeout">Tiempo de espera opcional para la consulta.</param>
        /// <param name="type">Tipo opcional de comando SQL.</param>
        /// <returns>Un objeto del tipo especificado o su valor predeterminado.</returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object? parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        /// <summary>
        /// Método interno para garantizar la auditoría de los cambios en las entidades.
        /// </summary>
        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
        }

        /// <summary>
        /// Estructura para ejecutar comandos SQL con Dapper en Entity Framework Core.
        /// </summary>
        public readonly struct DapperEFCoreCommand : IDisposable
        {
            /// <summary>
            /// Constructor del comando Dapper.
            /// </summary>
            /// <param name="context">Contexto de la base de datos.</param>
            /// <param name="text">Consulta SQL.</param>
            /// <param name="parameters">Parámetros opcionales.</param>
            /// <param name="timeout">Tiempo de espera opcional.</param>
            /// <param name="type">Tipo de comando SQL opcional.</param>
            /// <param name="ct">Token de cancelación.</param>
            public DapperEFCoreCommand(DbContext context, string text, object parameters, int? timeout, CommandType? type, CancellationToken ct)
            {
                var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                var commandType = type ?? CommandType.Text;
                var commandTimeout = timeout ?? context.Database.GetCommandTimeout() ?? 30;

                Definition = new CommandDefinition(
                    text,
                    parameters,
                    transaction,
                    commandTimeout,
                    commandType,
                    cancellationToken: ct
                );
            }

            /// <summary>
            /// Define los parámetros del comando SQL.
            /// </summary>
            public CommandDefinition Definition { get; }

            /// <summary>
            /// Método para liberar los recursos.
            /// </summary>
            public void Dispose()
            {
            }
        }
    }
}