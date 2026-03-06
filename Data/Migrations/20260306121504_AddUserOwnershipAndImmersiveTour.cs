using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnershipAndImmersiveTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmpresasFrequentes_Nome",
                table: "EmpresasFrequentes");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "StatusPendencias",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "MovimentacoesFinanceiras",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "FormasPagamento",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "EmpresasFrequentes",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CategoriasGasto",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CartoesCredito",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingConcluido",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "TourAtivo",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TourEtapa",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE CategoriasGasto " +
                "SET ApplicationUserId = COALESCE((SELECT Id FROM AspNetUsers ORDER BY CASE WHEN UPPER(Email) = 'ADMIN@FINANCES.COM' THEN 0 ELSE 1 END, Id LIMIT 1), ApplicationUserId) " +
                "WHERE ApplicationUserId = '';");

            migrationBuilder.Sql(
                "UPDATE FormasPagamento " +
                "SET ApplicationUserId = COALESCE((SELECT Id FROM AspNetUsers ORDER BY CASE WHEN UPPER(Email) = 'ADMIN@FINANCES.COM' THEN 0 ELSE 1 END, Id LIMIT 1), ApplicationUserId) " +
                "WHERE ApplicationUserId = '';");

            migrationBuilder.Sql(
                "UPDATE StatusPendencias " +
                "SET ApplicationUserId = COALESCE((SELECT Id FROM AspNetUsers ORDER BY CASE WHEN UPPER(Email) = 'ADMIN@FINANCES.COM' THEN 0 ELSE 1 END, Id LIMIT 1), ApplicationUserId) " +
                "WHERE ApplicationUserId = '';");

            migrationBuilder.Sql(
                "UPDATE EmpresasFrequentes " +
                "SET ApplicationUserId = COALESCE((SELECT Id FROM AspNetUsers ORDER BY CASE WHEN UPPER(Email) = 'ADMIN@FINANCES.COM' THEN 0 ELSE 1 END, Id LIMIT 1), ApplicationUserId) " +
                "WHERE ApplicationUserId = '';");

            migrationBuilder.Sql(
                "UPDATE CartoesCredito " +
                "SET ApplicationUserId = COALESCE((SELECT Id FROM AspNetUsers ORDER BY CASE WHEN UPPER(Email) = 'ADMIN@FINANCES.COM' THEN 0 ELSE 1 END, Id LIMIT 1), ApplicationUserId) " +
                "WHERE ApplicationUserId = '';");

            migrationBuilder.Sql(
                "UPDATE MovimentacoesFinanceiras " +
                "SET ApplicationUserId = COALESCE((SELECT Id FROM AspNetUsers WHERE UPPER(Email) = UPPER(MovimentacoesFinanceiras.Usuario) LIMIT 1), " +
                "(SELECT Id FROM AspNetUsers ORDER BY CASE WHEN UPPER(Email) = 'ADMIN@FINANCES.COM' THEN 0 ELSE 1 END, Id LIMIT 1), ApplicationUserId) " +
                "WHERE ApplicationUserId = '';");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasFrequentes_ApplicationUserId_Nome",
                table: "EmpresasFrequentes",
                columns: new[] { "ApplicationUserId", "Nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmpresasFrequentes_ApplicationUserId_Nome",
                table: "EmpresasFrequentes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "StatusPendencias");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "MovimentacoesFinanceiras");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "FormasPagamento");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "EmpresasFrequentes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CategoriasGasto");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CartoesCredito");

            migrationBuilder.DropColumn(
                name: "OnboardingConcluido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TourAtivo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TourEtapa",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasFrequentes_Nome",
                table: "EmpresasFrequentes",
                column: "Nome",
                unique: true);
        }
    }
}
