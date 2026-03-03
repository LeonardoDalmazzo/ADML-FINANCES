using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoLancamentoMovimentacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoLancamento",
                table: "MovimentacoesFinanceiras",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pagar");

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("04f75ddb-6d10-4b11-a73e-cf5b686eb8e9"),
                column: "TipoLancamento",
                value: "Pagar");

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("3e16f6af-838a-4cb8-9516-b8d76fe1ca8a"),
                column: "TipoLancamento",
                value: "Pagar");

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("7b589280-b00e-4c32-a78b-0f7dd03cd696"),
                column: "TipoLancamento",
                value: "Pagar");

            migrationBuilder.UpdateData(
                table: "MovimentacoesFinanceiras",
                keyColumn: "Id",
                keyValue: new Guid("bdebdc58-65c8-453b-962d-f9002dc81e81"),
                column: "TipoLancamento",
                value: "Pagar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoLancamento",
                table: "MovimentacoesFinanceiras");
        }
    }
}
