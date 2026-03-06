using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ADML_FINANCES.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<CartaoCredito> CartoesCredito => Set<CartaoCredito>();
    public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();
    public DbSet<EmpresaFrequente> EmpresasFrequentes => Set<EmpresaFrequente>();
    public DbSet<FormaPagamento> FormasPagamento => Set<FormaPagamento>();
    public DbSet<MovimentacaoFinanceira> MovimentacoesFinanceiras => Set<MovimentacaoFinanceira>();
    public DbSet<StatusPendencia> StatusPendencias => Set<StatusPendencia>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CategoriaGasto>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ApplicationUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(300).IsRequired();
        });

        builder.Entity<FormaPagamento>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ApplicationUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Cor).HasMaxLength(7).IsRequired();
            entity.Property(x => x.Situacao).IsRequired();
        });

        builder.Entity<CartaoCredito>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ApplicationUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Banco).HasMaxLength(120).IsRequired();
            entity.Property(x => x.LimiteDisponivel).HasColumnType("decimal(18,2)");
            entity.Property(x => x.LimiteEmUso).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DiaFechamento).IsRequired();
            entity.Property(x => x.DiaVencimento).IsRequired();
            entity.HasIndex(x => x.FormaPagamentoId).IsUnique();

            entity.HasOne(x => x.FormaPagamento)
                .WithMany()
                .HasForeignKey(x => x.FormaPagamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<EmpresaFrequente>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ApplicationUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => new { x.ApplicationUserId, x.Nome }).IsUnique();
        });

        builder.Entity<StatusPendencia>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ApplicationUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Cor).HasMaxLength(7).IsRequired();
        });

        builder.Entity<MovimentacaoFinanceira>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ApplicationUserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.TipoLancamento).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Usuario).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Observacao).HasMaxLength(300);

            entity.HasOne(x => x.CategoriaGasto)
                .WithMany()
                .HasForeignKey(x => x.CategoriaGastoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.FormaPagamento)
                .WithMany()
                .HasForeignKey(x => x.FormaPagamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.StatusPendencia)
                .WithMany()
                .HasForeignKey(x => x.StatusPendenciaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EmpresaFrequente)
                .WithMany()
                .HasForeignKey(x => x.EmpresaFrequenteId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
