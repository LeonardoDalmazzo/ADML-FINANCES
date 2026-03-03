using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresasFrequentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmpresaFrequenteId",
                table: "MovimentacoesFinanceiras",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmpresasFrequentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasFrequentes", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("04f75ddb-6d10-4b11-a73e-cf5b686eb8e9"),
                column: "EmpresaFrequenteId",
                value: null);

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("3e16f6af-838a-4cb8-9516-b8d76fe1ca8a"),
                column: "EmpresaFrequenteId",
                value: null);

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("7b589280-b00e-4c32-a78b-0f7dd03cd696"),
                column: "EmpresaFrequenteId",
                value: null);

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("bdebdc58-65c8-453b-962d-f9002dc81e81"),
                column: "EmpresaFrequenteId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesFinanceiras_EmpresaFrequenteId",
                table: "MovimentacoesFinanceiras",
                column: "EmpresaFrequenteId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasFrequentes_Nome",
                table: "EmpresasFrequentes",
                column: "Nome",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesFinanceiras_EmpresasFrequentes_EmpresaFrequenteId",
                table: "MovimentacoesFinanceiras",
                column: "EmpresaFrequenteId",
                principalTable: "EmpresasFrequentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesFinanceiras_EmpresasFrequentes_EmpresaFrequenteId",
                table: "MovimentacoesFinanceiras");

            migrationBuilder.DropTable(
                name: "EmpresasFrequentes");

            migrationBuilder.DropIndex(
                name: "IX_MovimentacoesFinanceiras_EmpresaFrequenteId",
                table: "MovimentacoesFinanceiras");

            migrationBuilder.DropColumn(
                name: "EmpresaFrequenteId",
                table: "MovimentacoesFinanceiras");
        }
    }
}
