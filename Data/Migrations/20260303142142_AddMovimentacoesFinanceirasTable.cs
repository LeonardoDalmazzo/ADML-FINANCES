using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMovimentacoesFinanceirasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovimentacoesFinanceiras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataLancamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Usuario = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    CategoriaGastoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FormaPagamentoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StatusPendenciaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesFinanceiras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesFinanceiras_CategoriasGasto_CategoriaGastoId",
                        column: x => x.CategoriaGastoId,
                        principalTable: "CategoriasGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimentacoesFinanceiras_FormasPagamento_FormaPagamentoId",
                        column: x => x.FormaPagamentoId,
                        principalTable: "FormasPagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimentacoesFinanceiras_StatusPendencias_StatusPendenciaId",
                        column: x => x.StatusPendenciaId,
                        principalTable: "StatusPendencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MovimentacoesFinanceiras",
                columns: new[] { "Id", "CategoriaGastoId", "DataLancamento", "DataPagamento", "DataVencimento", "Descricao", "FormaPagamentoId", "Observacao", "StatusPendenciaId", "Usuario", "Valor" },
                values: new object[,]
                {
                    { new Guid("04f75ddb-6d10-4b11-a73e-cf5b686eb8e9"), new Guid("97cdac35-773f-4725-a48f-4e7403ef0563"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Curso de investimentos", new Guid("19c2abfd-e8bc-4450-8ecb-fec0f5f236b4"), "Parcela unica.", new Guid("8ea5d574-a363-47be-97b8-9ef15ce0fb1d"), "admin@finances.com", 297.90m },
                    { new Guid("3e16f6af-838a-4cb8-9516-b8d76fe1ca8a"), new Guid("e56c9989-8116-4f86-ba0a-86d0f0e4c8d7"), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Farmacia", new Guid("ad293e8f-b852-443a-8dd4-6f39a7baafe1"), "Medicamentos continuos.", new Guid("c17a7b95-3240-4528-b062-dd0712bdd3c4"), "admin@finances.com", 124.77m },
                    { new Guid("7b589280-b00e-4c32-a78b-0f7dd03cd696"), new Guid("34fc3fa5-fe66-4f3f-9966-6a87248eaa75"), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Assinatura streaming", new Guid("ad293e8f-b852-443a-8dd4-6f39a7baafe1"), "Recorrente.", new Guid("7075cd4c-2f4f-499c-b538-d0c9124b1a8e"), "admin@finances.com", 39.90m },
                    { new Guid("bdebdc58-65c8-453b-962d-f9002dc81e81"), new Guid("14f3d711-40cd-44c3-8697-9159e50dce53"), new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mercado semanal", new Guid("ad293e8f-b852-443a-8dd4-6f39a7baafe1"), "Compras da semana.", new Guid("8b3d30f4-d44e-45f7-b996-d8400e0a32e0"), "admin@finances.com", 286.40m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesFinanceiras_CategoriaGastoId",
                table: "MovimentacoesFinanceiras",
                column: "CategoriaGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesFinanceiras_FormaPagamentoId",
                table: "MovimentacoesFinanceiras",
                column: "FormaPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesFinanceiras_StatusPendenciaId",
                table: "MovimentacoesFinanceiras",
                column: "StatusPendenciaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimentacoesFinanceiras");
        }
    }
}
