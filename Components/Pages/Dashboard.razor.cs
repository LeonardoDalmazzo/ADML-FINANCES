using ADML_FINANCES.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ADML_FINANCES.Components.Pages;

public partial class Dashboard : ComponentBase
{
    [Inject] private ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private string? usuarioAtual;
    private List<MovimentacaoFinanceira> movimentacoes = [];

    private decimal TotalPagar => movimentacoes.Where(x => x.TipoLancamento == "Pagar").Sum(x => x.Valor);
    private decimal TotalReceber => movimentacoes.Where(x => x.TipoLancamento == "Receber").Sum(x => x.Valor);
    private decimal SaldoProjetado => TotalReceber - TotalPagar;
    private decimal TotalVencidos => movimentacoes
        .Where(x => x.DataVencimento.Date < DateTime.Today && (x.DataPagamento == null || x.StatusPendencia?.Nome is not ("Pago" or "Recebido")))
        .Sum(x => x.Valor);
    private decimal TotalVencendo7Dias => movimentacoes
        .Where(x => x.DataVencimento.Date >= DateTime.Today && x.DataVencimento.Date <= DateTime.Today.AddDays(7))
        .Where(x => x.DataPagamento == null || x.StatusPendencia?.Nome is not ("Pago" or "Recebido"))
        .Sum(x => x.Valor);
    private decimal TaxaLiquidacao
    {
        get
        {
            if (movimentacoes.Count == 0)
            {
                return 0;
            }

            var liquidados = movimentacoes.Count(x => x.StatusPendencia?.Nome is "Pago" or "Recebido");
            return (decimal)liquidados / movimentacoes.Count * 100m;
        }
    }

    private decimal TicketMedioPagar
    {
        get
        {
            var itens = movimentacoes.Where(x => x.TipoLancamento == "Pagar").ToList();
            return itens.Count == 0 ? 0 : itens.Average(x => x.Valor);
        }
    }

    private decimal TicketMedioReceber
    {
        get
        {
            var itens = movimentacoes.Where(x => x.TipoLancamento == "Receber").ToList();
            return itens.Count == 0 ? 0 : itens.Average(x => x.Valor);
        }
    }

    private List<ItemRanking> TopCategoriasPagar => movimentacoes
        .Where(x => x.TipoLancamento == "Pagar")
        .GroupBy(x => x.CategoriaGasto?.Nome ?? "Sem categoria")
        .Select(x => new ItemRanking(x.Key, x.Sum(y => y.Valor)))
        .OrderByDescending(x => x.Valor)
        .Take(5)
        .ToList();

    private List<ItemRanking> TopEmpresas => movimentacoes
        .GroupBy(x => x.EmpresaFrequente?.Nome ?? "Sem empresa")
        .Select(x => new ItemRanking(x.Key, x.Sum(y => y.Valor)))
        .OrderByDescending(x => x.Valor)
        .Take(5)
        .ToList();

    private List<MovimentacaoFinanceira> ProximosVencimentos => movimentacoes
        .Where(x => x.DataVencimento.Date >= DateTime.Today && x.DataVencimento.Date <= DateTime.Today.AddDays(10))
        .Where(x => x.DataPagamento == null || x.StatusPendencia?.Nome is not ("Pago" or "Recebido"))
        .OrderBy(x => x.DataVencimento)
        .ThenByDescending(x => x.Valor)
        .Take(6)
        .ToList();

    private List<string> Insights
    {
        get
        {
            var resultado = new List<string>();

            if (TotalVencidos > 0)
            {
                resultado.Add($"Voce tem {TotalVencidos:C} em atrasos. Priorize os itens urgentes.");
            }

            if (TotalVencendo7Dias > 0)
            {
                resultado.Add($"{TotalVencendo7Dias:C} vencem nos proximos 7 dias.");
            }

            if (SaldoProjetado < 0)
            {
                resultado.Add("Seu saldo projetado esta negativo. Reforce recebimentos ou reduza despesas.");
            }

            var maiorCategoria = TopCategoriasPagar.FirstOrDefault();
            if (maiorCategoria is not null)
            {
                resultado.Add($"Maior peso atual: {maiorCategoria.Nome} ({maiorCategoria.Valor:C}).");
            }

            if (TaxaLiquidacao >= 75)
            {
                resultado.Add("Taxa de liquidacao alta. Controle financeiro em bom ritmo.");
            }

            return resultado;
        }
    }

    private List<ItemMesResumo> ResumoMensal => Enumerable.Range(0, 6)
        .Select(offset => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-offset))
        .OrderBy(x => x)
        .Select(mes =>
        {
            var itensMes = movimentacoes.Where(x => x.DataVencimento.Year == mes.Year && x.DataVencimento.Month == mes.Month);
            var pagar = itensMes.Where(x => x.TipoLancamento == "Pagar").Sum(x => x.Valor);
            var receber = itensMes.Where(x => x.TipoLancamento == "Receber").Sum(x => x.Valor);
            return new ItemMesResumo(mes.ToString("MM/yyyy"), pagar, receber);
        })
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        usuarioAtual = authState.User.Identity?.Name;

        var query = DbContext.MovimentacoesFinanceiras
            .Include(x => x.CategoriaGasto)
            .Include(x => x.EmpresaFrequente)
            .Include(x => x.StatusPendencia)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(usuarioAtual))
        {
            query = query.Where(x => x.Usuario == usuarioAtual);
        }

        movimentacoes = await query.ToListAsync();
    }

    private sealed record ItemRanking(string Nome, decimal Valor);

    private sealed record ItemMesResumo(string Mes, decimal TotalPagar, decimal TotalReceber)
    {
        public decimal Saldo => TotalReceber - TotalPagar;
    }
}
