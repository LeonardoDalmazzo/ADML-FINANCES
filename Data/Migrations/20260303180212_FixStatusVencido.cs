using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixStatusVencido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE MovimentacoesFinanceiras
                SET StatusPendenciaId = (
                    SELECT Id FROM StatusPendencias WHERE Nome = 'Vencido' LIMIT 1
                )
                WHERE StatusPendenciaId = (
                    SELECT Id FROM StatusPendencias WHERE Nome = 'Vencidado' LIMIT 1
                )
                AND EXISTS (SELECT 1 FROM StatusPendencias WHERE Nome = 'Vencido')
                AND EXISTS (SELECT 1 FROM StatusPendencias WHERE Nome = 'Vencidado');
                """);

            migrationBuilder.UpdateData(
                table: "StatusPendencias",
                keyColumn: "Id",
                keyValue: new Guid("744768a3-0205-4da8-b51a-7a91684a35f3"),
                column: "Nome",
                value: "Vencido");

            migrationBuilder.Sql("""
                DELETE FROM StatusPendencias
                WHERE Nome = 'Vencidado'
                AND Id <> '744768a3-0205-4da8-b51a-7a91684a35f3';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "StatusPendencias",
                keyColumn: "Id",
                keyValue: new Guid("744768a3-0205-4da8-b51a-7a91684a35f3"),
                column: "Nome",
                value: "Vencidado");
        }
    }
}
