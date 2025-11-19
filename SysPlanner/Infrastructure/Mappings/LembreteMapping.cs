using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysPlanner.Infrastructure.Persistance;

namespace SysPlanner.Infrastructure.Mappings
{
    public class LembreteMapping : IEntityTypeConfiguration<Lembrete>
    {
        public void Configure(EntityTypeBuilder<Lembrete> builder)
        {
            builder.ToTable("Lembretes");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Titulo)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(l => l.Descricao)
                   .HasMaxLength(500);

            builder.Property(l => l.Data)
                   .IsRequired();

            builder.Property(l => l.Prioridade)
                   .IsRequired();

            builder.Property(l => l.Categoria)
                   .IsRequired();

            builder.Property(l => l.Concluido)
                   .HasMaxLength(1)
                   .HasDefaultValue("N")
                   .IsRequired();

            builder.HasOne(l => l.Usuario)
                   .WithMany(u => u.Lembretes)
                   .HasForeignKey(l => l.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
