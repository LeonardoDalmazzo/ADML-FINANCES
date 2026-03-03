using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADML_FINANCES.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTipoLancamentoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE MovimentacoesFinanceiras
                SET TipoLancamento = 'Pagar'
                WHERE TipoLancamento IS NULL OR TRIM(TipoLancamento) = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE MovimentacoesFinanceiras
                SET TipoLancamento = ''
                WHERE TipoLancamento = 'Pagar';
                """);
        }
    }
}
