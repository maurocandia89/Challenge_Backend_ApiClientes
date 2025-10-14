using Microsoft.EntityFrameworkCore;
using ApiClientes.Models;

namespace ApiClientes.Data;

    public class ClienteContext : DbContext
    {
        public ClienteContext(DbContextOptions<ClienteContext> options)
            : base(options)
        {
        }

        // Esta propiedad representa la tabla "Clientes" en la base de datos
        public DbSet<Cliente> Clientes { get; set; }

        // Opcional: Configuración adicional del modelo (ej. índices, restricciones)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ejemplo: Asegurar que el CUIT sea único (una buena práctica)
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CUIT)
                .IsUnique();
        }
    }
