using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCartoesCredito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartoesCredito",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Banco = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    LimiteDisponivel = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LimiteEmUso = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiaFechamento = table.Column<int>(type: "INTEGER", nullable: false),
                    DiaVencimento = table.Column<int>(type: "INTEGER", nullable: false),
                    FormaPagamentoId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartoesCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartoesCredito_FormasPagamento_FormaPagamentoId",
                        column: x => x.FormaPagamentoId,
                        principalTable: "FormasPagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartoesCredito_FormaPagamentoId",
                table: "CartoesCredito",
                column: "FormaPagamentoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartoesCredito");
        }
    }
}
