using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriasGastoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasGasto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasGasto", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CategoriasGasto",
                columns: new[] { "Id", "Descricao", "Nome" },
                values: new object[,]
                {
                    { new Guid("14f3d711-40cd-44c3-8697-9159e50dce53"), "Compras de mercado e refeições.", "Alimentação" },
                    { new Guid("34fc3fa5-fe66-4f3f-9966-6a87248eaa75"), "Despesas com lazer e entretenimento.", "Lazer" },
                    { new Guid("97cdac35-773f-4725-a48f-4e7403ef0563"), "Cursos, livros e materiais de estudo.", "Estudo" },
                    { new Guid("e56c9989-8116-4f86-ba0a-86d0f0e4c8d7"), "Consultas, exames, farmácias e planos.", "Saúde" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriasGasto");
        }
    }
}
