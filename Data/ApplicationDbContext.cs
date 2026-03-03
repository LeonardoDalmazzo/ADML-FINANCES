using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ADML_FINANCES.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
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
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(300).IsRequired();

            entity.HasData(
                new CategoriaGasto { Id = Guid.Parse("34fc3fa5-fe66-4f3f-9966-6a87248eaa75"), Nome = "Lazer", Descricao = "Despesas com lazer e entretenimento." },
                new CategoriaGasto { Id = Guid.Parse("97cdac35-773f-4725-a48f-4e7403ef0563"), Nome = "Estudo", Descricao = "Cursos, livros e materiais de estudo." },
                new CategoriaGasto { Id = Guid.Parse("14f3d711-40cd-44c3-8697-9159e50dce53"), Nome = "Alimenta\u00E7\u00E3o", Descricao = "Compras de mercado e refei\u00E7\u00F5es." },
                new CategoriaGasto { Id = Guid.Parse("e56c9989-8116-4f86-ba0a-86d0f0e4c8d7"), Nome = "Sa\u00FAde", Descricao = "Consultas, exames, farm\u00E1cias e planos." }
            );
        });

        builder.Entity<FormaPagamento>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Cor).HasMaxLength(7).IsRequired();
            entity.Property(x => x.Situacao).IsRequired();

            entity.HasData(
                new FormaPagamento
                {
                    Id = Guid.Parse("ad293e8f-b852-443a-8dd4-6f39a7baafe1"),
                    Nome = "Pix",
                    Descricao = "Transferencia instantanea.",
                    Cor = "#0d6efd",
                    Situacao = true
                },
                new FormaPagamento
                {
                    Id = Guid.Parse("19c2abfd-e8bc-4450-8ecb-fec0f5f236b4"),
                    Nome = "Cartao de credito",
                    Descricao = "Pagamento via cartao com faturamento.",
                    Cor = "#6f42c1",
                    Situacao = true
                }
            );
        });

        builder.Entity<EmpresaFrequente>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Nome).IsUnique();
        });

        builder.Entity<StatusPendencia>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Cor).HasMaxLength(7).IsRequired();

            entity.HasData(
                new StatusPendencia { Id = Guid.Parse("8b3d30f4-d44e-45f7-b996-d8400e0a32e0"), Nome = "Recebido", Cor = "#3b82f6" },
                new StatusPendencia { Id = Guid.Parse("7075cd4c-2f4f-499c-b538-d0c9124b1a8e"), Nome = "Pago", Cor = "#22c55e" },
                new StatusPendencia { Id = Guid.Parse("8ea5d574-a363-47be-97b8-9ef15ce0fb1d"), Nome = "Aguardando", Cor = "#f59e0b" },
                new StatusPendencia { Id = Guid.Parse("744768a3-0205-4da8-b51a-7a91684a35f3"), Nome = "Vencido", Cor = "#ef4444" },
                new StatusPendencia { Id = Guid.Parse("c17a7b95-3240-4528-b062-dd0712bdd3c4"), Nome = "Urgente", Cor = "#dc2626" }
            );
        });

        builder.Entity<MovimentacaoFinanceira>(entity =>
        {
            entity.HasKey(x => x.Id);
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

            entity.HasData(
                new MovimentacaoFinanceira
                {
                    Id = Guid.Parse("04f75ddb-6d10-4b11-a73e-cf5b686eb8e9"),
                    TipoLancamento = "Pagar",
                    Descricao = "Curso de investimentos",
                    Valor = 297.90m,
                    DataLancamento = new DateTime(2026, 3, 1),
                    DataVencimento = new DateTime(2026, 3, 10),
                    DataPagamento = null,
                    Usuario = "admin@finances.com",
                    Observacao = "Parcela unica.",
                    CategoriaGastoId = Guid.Parse("97cdac35-773f-4725-a48f-4e7403ef0563"),
                    FormaPagamentoId = Guid.Parse("19c2abfd-e8bc-4450-8ecb-fec0f5f236b4"),
                    StatusPendenciaId = Guid.Parse("8ea5d574-a363-47be-97b8-9ef15ce0fb1d")
                },
                new MovimentacaoFinanceira
                {
                    Id = Guid.Parse("7b589280-b00e-4c32-a78b-0f7dd03cd696"),
                    TipoLancamento = "Pagar",
                    Descricao = "Assinatura streaming",
                    Valor = 39.90m,
                    DataLancamento = new DateTime(2026, 3, 2),
                    DataVencimento = new DateTime(2026, 3, 5),
                    DataPagamento = new DateTime(2026, 3, 3),
                    Usuario = "admin@finances.com",
                    Observacao = "Recorrente.",
                    CategoriaGastoId = Guid.Parse("34fc3fa5-fe66-4f3f-9966-6a87248eaa75"),
                    FormaPagamentoId = Guid.Parse("ad293e8f-b852-443a-8dd4-6f39a7baafe1"),
                    StatusPendenciaId = Guid.Parse("7075cd4c-2f4f-499c-b538-d0c9124b1a8e")
                },
                new MovimentacaoFinanceira
                {
                    Id = Guid.Parse("3e16f6af-838a-4cb8-9516-b8d76fe1ca8a"),
                    TipoLancamento = "Pagar",
                    Descricao = "Farmacia",
                    Valor = 124.77m,
                    DataLancamento = new DateTime(2026, 3, 2),
                    DataVencimento = new DateTime(2026, 3, 4),
                    DataPagamento = null,
                    Usuario = "admin@finances.com",
                    Observacao = "Medicamentos continuos.",
                    CategoriaGastoId = Guid.Parse("e56c9989-8116-4f86-ba0a-86d0f0e4c8d7"),
                    FormaPagamentoId = Guid.Parse("ad293e8f-b852-443a-8dd4-6f39a7baafe1"),
                    StatusPendenciaId = Guid.Parse("c17a7b95-3240-4528-b062-dd0712bdd3c4")
                },
                new MovimentacaoFinanceira
                {
                    Id = Guid.Parse("bdebdc58-65c8-453b-962d-f9002dc81e81"),
                    TipoLancamento = "Pagar",
                    Descricao = "Mercado semanal",
                    Valor = 286.40m,
                    DataLancamento = new DateTime(2026, 3, 3),
                    DataVencimento = new DateTime(2026, 3, 8),
                    DataPagamento = null,
                    Usuario = "admin@finances.com",
                    Observacao = "Compras da semana.",
                    CategoriaGastoId = Guid.Parse("14f3d711-40cd-44c3-8697-9159e50dce53"),
                    FormaPagamentoId = Guid.Parse("ad293e8f-b852-443a-8dd4-6f39a7baafe1"),
                    StatusPendenciaId = Guid.Parse("8b3d30f4-d44e-45f7-b996-d8400e0a32e0")
                }
            );
        });
    }
}
