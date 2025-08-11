using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaPetrobras.Models;

namespace SistemaPetrobras.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Morador> Moradores { get; set; }
        public DbSet<Aviso> Avisos { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configurações adicionais se necessário
            builder.Entity<Morador>()
                .HasIndex(m => m.CPF)
                .IsUnique();
                
            builder.Entity<Morador>()
                .HasIndex(m => m.Email)
                .IsUnique();
        }
    }
}

