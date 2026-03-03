using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusPendenciasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatusPendencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cor = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusPendencias", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "StatusPendencias",
                columns: new[] { "Id", "Cor", "Nome" },
                values: new object[,]
                {
                    { new Guid("7075cd4c-2f4f-499c-b538-d0c9124b1a8e"), "#22c55e", "Pago" },
                    { new Guid("744768a3-0205-4da8-b51a-7a91684a35f3"), "#ef4444", "Vencidado" },
                    { new Guid("8b3d30f4-d44e-45f7-b996-d8400e0a32e0"), "#3b82f6", "Recebido" },
                    { new Guid("8ea5d574-a363-47be-97b8-9ef15ce0fb1d"), "#f59e0b", "Aguardando" },
                    { new Guid("c17a7b95-3240-4528-b062-dd0712bdd3c4"), "#dc2626", "Urgente" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusPendencias");
        }
    }
}
