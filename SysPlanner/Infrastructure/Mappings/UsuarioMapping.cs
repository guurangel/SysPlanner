using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysPlanner.Infrastructure.Persistance;

namespace SysPlanner.Infrastructure.Mappings
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder
                .HasIndex(u => u.Email).IsUnique();
            builder
                .HasIndex(u => u.Cpf).IsUnique();

            builder.Property(u => u.Nome)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(u => u.Senha)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Cpf)
                   .HasMaxLength(11)
                   .IsRequired()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.OwnsOne(u => u.Endereco, endereco =>
            {
                endereco.Property(e => e.Rua)
                    .HasMaxLength(150)
                    .IsRequired();

                endereco.Property(e => e.Numero)
                    .HasMaxLength(20)
                    .IsRequired();

                endereco.Property(e => e.Complemento)
                    .HasMaxLength(60);

                endereco.Property(e => e.Bairro)
                    .HasMaxLength(100)
                    .IsRequired();

                endereco.Property(e => e.Cidade)
                    .HasMaxLength(100)
                    .IsRequired();

                endereco.Property(e => e.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(2)
                    .IsRequired();

                endereco.Property(e => e.Cep)
                    .HasMaxLength(8)
                    .IsRequired();

            });

            builder
                .HasMany(u => u.Lembretes)
                .WithOne(l => l.Usuario)
                .HasForeignKey(l => l.UsuarioId);
        }
    }
}