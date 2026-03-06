using Microsoft.EntityFrameworkCore;

namespace ADML_FINANCES.Data;

public static class CartaoCreditoLimiteHelper
{
    public static async Task RecalcularLimitesEmUsoAsync(ApplicationDbContext dbContext, string? applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            return;
        }

        var cartoes = await dbContext.CartoesCredito
            .Where(x => x.ApplicationUserId == applicationUserId)
            .ToListAsync();
        if (cartoes.Count == 0)
        {
            return;
        }

        var query = dbContext.MovimentacoesFinanceiras
            .Include(x => x.StatusPendencia)
            .Where(x => x.TipoLancamento == "Pagar")
            .AsQueryable();

        query = query.Where(x => x.ApplicationUserId == applicationUserId);

        var movimentacoes = await query.ToListAsync();

        foreach (var cartao in cartoes)
        {
            var limiteTotal = cartao.LimiteDisponivel + cartao.LimiteEmUso;
            var limiteEmUsoCalculado = movimentacoes
                .Where(x => x.FormaPagamentoId == cartao.FormaPagamentoId)
                .Where(x => !EstaLiquidado(x))
                .Sum(x => x.Valor);

            cartao.LimiteEmUso = limiteEmUsoCalculado;
            cartao.LimiteDisponivel = Math.Max(limiteTotal - limiteEmUsoCalculado, 0);
        }

        await dbContext.SaveChangesAsync();
    }

    private static bool EstaLiquidado(MovimentacaoFinanceira item)
    {
        return item.StatusPendencia?.Nome is "Pago";
    }
}
