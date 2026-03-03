using ADML_FINANCES.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ADML_FINANCES.Components.Pages;

public partial class Visualizacao : ComponentBase
{
    [Inject] private ApplicationDbContext DbContext { get; set; } = default!;

    private List<MovimentacaoFinanceira> movimentacoes = [];
    private List<CategoriaGasto> categorias = [];
    private List<FormaPagamento> formasPagamento = [];
    private List<StatusPendencia> statusPendencias = [];
    private List<string> usuarios = [];

    private string? pesquisa;
    private Guid? categoriaId;
    private Guid? formaPagamentoId;
    private Guid? statusId;
    private string? usuarioSelecionado;
    private DateTime? dataInicio;
    private DateTime? dataFim;
    private string ordenarPor = "vencimento";
    private string direcaoOrdenacao = "asc";

    private bool mostrarDescricao = true;
    private bool mostrarCategoria = true;
    private bool mostrarForma = true;
    private bool mostrarStatus = true;
    private bool mostrarValor = true;
    private bool mostrarVencimento = true;
    private bool mostrarPagamento = true;
    private bool mostrarUsuario = true;

    private int ColSpan =>
        (mostrarDescricao ? 1 : 0) +
        (mostrarCategoria ? 1 : 0) +
        (mostrarForma ? 1 : 0) +
        (mostrarStatus ? 1 : 0) +
        (mostrarValor ? 1 : 0) +
        (mostrarVencimento ? 1 : 0) +
        (mostrarPagamento ? 1 : 0) +
        (mostrarUsuario ? 1 : 0);

    private decimal TotalFiltrado => movimentacoes.Sum(x => x.Valor);
    private decimal TotalPago => movimentacoes.Where(x => x.StatusPendencia?.Nome == "Pago").Sum(x => x.Valor);
    private decimal TotalPendente => movimentacoes.Where(x => x.StatusPendencia?.Nome != "Pago").Sum(x => x.Valor);

    protected override async Task OnInitializedAsync()
    {
        categorias = await DbContext.CategoriasGasto.OrderBy(x => x.Nome).ToListAsync();
        formasPagamento = await DbContext.FormasPagamento.OrderBy(x => x.Nome).ToListAsync();
        statusPendencias = await DbContext.StatusPendencias.OrderBy(x => x.Nome).ToListAsync();
        usuarios = await DbContext.MovimentacoesFinanceiras
            .Select(x => x.Usuario)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        await BuscarAsync();
    }

    private async Task BuscarAsync()
    {
        var query = DbContext.MovimentacoesFinanceiras
            .Include(x => x.CategoriaGasto)
            .Include(x => x.FormaPagamento)
            .Include(x => x.StatusPendencia)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            query = query.Where(x =>
                x.Descricao.Contains(termo) ||
                (x.Observacao != null && x.Observacao.Contains(termo)));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaGastoId == categoriaId.Value);
        }

        if (formaPagamentoId.HasValue)
        {
            query = query.Where(x => x.FormaPagamentoId == formaPagamentoId.Value);
        }

        if (statusId.HasValue)
        {
            query = query.Where(x => x.StatusPendenciaId == statusId.Value);
        }

        if (!string.IsNullOrWhiteSpace(usuarioSelecionado))
        {
            query = query.Where(x => x.Usuario == usuarioSelecionado);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(x => x.DataVencimento >= dataInicio.Value.Date);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(x => x.DataVencimento <= dataFim.Value.Date);
        }

        movimentacoes = await AplicarOrdenacao(query).ToListAsync();
    }

    private async Task LimparAsync()
    {
        pesquisa = null;
        categoriaId = null;
        formaPagamentoId = null;
        statusId = null;
        usuarioSelecionado = null;
        dataInicio = null;
        dataFim = null;
        ordenarPor = "vencimento";
        direcaoOrdenacao = "asc";
        await BuscarAsync();
    }

    private IQueryable<MovimentacaoFinanceira> AplicarOrdenacao(IQueryable<MovimentacaoFinanceira> query)
    {
        var desc = string.Equals(direcaoOrdenacao, "desc", StringComparison.OrdinalIgnoreCase);
        var campo = (ordenarPor ?? "vencimento").Trim().ToLowerInvariant();

        return campo switch
        {
            "descricao" => desc
                ? query.OrderByDescending(x => x.Descricao).ThenBy(x => x.DataVencimento)
                : query.OrderBy(x => x.Descricao).ThenBy(x => x.DataVencimento),
            "valor" => desc
                ? query.OrderByDescending(x => x.Valor).ThenBy(x => x.DataVencimento)
                : query.OrderBy(x => x.Valor).ThenBy(x => x.DataVencimento),
            "status" => desc
                ? query.OrderByDescending(x => x.StatusPendencia!.Nome).ThenBy(x => x.DataVencimento)
                : query.OrderBy(x => x.StatusPendencia!.Nome).ThenBy(x => x.DataVencimento),
            _ => desc
                ? query.OrderByDescending(x => x.DataVencimento).ThenBy(x => x.Descricao)
                : query.OrderBy(x => x.DataVencimento).ThenBy(x => x.Descricao)
        };
    }
}
