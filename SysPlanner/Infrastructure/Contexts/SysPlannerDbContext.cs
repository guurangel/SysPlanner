using Microsoft.EntityFrameworkCore;
using SysPlanner.Infrastructure.Persistance;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using SysPlanner.Infrastructure.Mappings;

namespace SysPlanner.Infrastructure.Contexts
{
    public class SysPlannerDbContext : DbContext
    {
        public SysPlannerDbContext(DbContextOptions<SysPlannerDbContext> options)
            : base(options)
        { }

        public DbSet<Lembrete> Lembretes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LembreteMapping());
            modelBuilder.ApplyConfiguration(new UsuarioMapping());
        }
    }
}