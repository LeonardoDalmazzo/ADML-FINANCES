using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormasPagamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Cor = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Situacao = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPagamento", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FormasPagamento",
                columns: new[] { "Id", "Cor", "Descricao", "Nome", "Situacao" },
                values: new object[,]
                {
                    { new Guid("19c2abfd-e8bc-4450-8ecb-fec0f5f236b4"), "#6f42c1", "Pagamento via cartao com faturamento.", "Cartao de credito", true },
                    { new Guid("ad293e8f-b852-443a-8dd4-6f39a7baafe1"), "#0d6efd", "Transferencia instantanea.", "Pix", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormasPagamento");
        }
    }
}
