using Microsoft.EntityFrameworkCore;
using ApiClientes.Models;

namespace ApiClientes.Data;

    public class ClienteContext : DbContext
    {
        public ClienteContext(DbContextOptions<ClienteContext> options)
            : base(options)
        {
        }
        public DbSet<Cliente> Clientes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CUIT)
                .IsUnique();
        }
    }
