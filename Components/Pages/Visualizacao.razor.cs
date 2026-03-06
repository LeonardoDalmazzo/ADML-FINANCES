using ADML_FINANCES.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Globalization;
using System.Text;

namespace ADML_FINANCES.Components.Pages;

public partial class Visualizacao : ComponentBase
{
    [Inject] private ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private TourProgressService TourProgressService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private List<MovimentacaoFinanceira> movimentacoes = [];
    private List<EmpresaFrequente> empresasFrequentes = [];
    private List<CategoriaGasto> categorias = [];
    private List<FormaPagamento> formasPagamento = [];
    private List<StatusPendencia> statusPendencias = [];

    private string? usuarioAtual;
    private string? applicationUserId;
    private string? pesquisa;
    private Guid? categoriaId;
    private Guid? formaPagamentoId;
    private Guid? statusId;
    private string? tipoLancamentoFiltro;
    private string? empresaSelecionada;
    private DateTime? dataInicio;
    private DateTime? dataFim;
    private string ordenarPor = "vencimento";
    private string direcaoOrdenacao = "asc";

    private bool mostrarTipo = true;
    private bool mostrarDescricao = true;
    private bool mostrarEmpresa = true;
    private bool mostrarCategoria = true;
    private bool mostrarForma = true;
    private bool mostrarStatus = true;
    private bool mostrarValor = true;
    private bool mostrarVencimento = true;
    private bool mostrarPagamento = true;

    private Guid? editandoId;
    private string editandoTipoLancamento = "Pagar";
    private string editandoDescricao = string.Empty;
    private string editandoEmpresaNome = string.Empty;
    private decimal editandoValor;
    private DateTime editandoDataVencimento;
    private DateTime? editandoDataPagamento;
    private Guid editandoCategoriaId;
    private Guid editandoFormaPagamentoId;
    private Guid editandoStatusId;

    private int ColSpan =>
        (mostrarTipo ? 1 : 0) +
        (mostrarDescricao ? 1 : 0) +
        (mostrarEmpresa ? 1 : 0) +
        (mostrarCategoria ? 1 : 0) +
        (mostrarForma ? 1 : 0) +
        (mostrarStatus ? 1 : 0) +
        (mostrarValor ? 1 : 0) +
        (mostrarVencimento ? 1 : 0) +
        (mostrarPagamento ? 1 : 0) +
        1;

    private decimal TotalPagar => movimentacoes.Where(x => x.TipoLancamento == "Pagar").Sum(x => x.Valor);
    private decimal TotalReceber => movimentacoes.Where(x => x.TipoLancamento == "Receber").Sum(x => x.Valor);
    private decimal SaldoPrevisto => TotalReceber - TotalPagar;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        usuarioAtual = authState.User.Identity?.Name;
        applicationUserId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

        await NormalizarTiposLancamentoAsync();

        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            return;
        }

        empresasFrequentes = await DbContext.EmpresasFrequentes
            .Where(x => x.ApplicationUserId == applicationUserId)
            .OrderBy(x => x.Nome)
            .ToListAsync();
        categorias = await DbContext.CategoriasGasto
            .Where(x => x.ApplicationUserId == applicationUserId)
            .OrderBy(x => x.Nome)
            .ToListAsync();
        formasPagamento = await DbContext.FormasPagamento
            .Where(x => x.ApplicationUserId == applicationUserId)
            .OrderBy(x => x.Nome)
            .ToListAsync();
        statusPendencias = await DbContext.StatusPendencias
            .Where(x => x.ApplicationUserId == applicationUserId)
            .OrderBy(x => x.Nome)
            .ToListAsync();

        await BuscarAsync();
    }

    private async Task BuscarAsync()
    {
        var query = DbContext.MovimentacoesFinanceiras
            .Include(x => x.CategoriaGasto)
            .Include(x => x.EmpresaFrequente)
            .Include(x => x.FormaPagamento)
            .Include(x => x.StatusPendencia)
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            movimentacoes = [];
            return;
        }

        query = query.Where(x => x.ApplicationUserId == applicationUserId);

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            query = query.Where(x =>
                x.Descricao.Contains(termo) ||
                (x.EmpresaFrequente != null && x.EmpresaFrequente.Nome.Contains(termo)) ||
                (x.Observacao != null && x.Observacao.Contains(termo)));
        }

        if (!string.IsNullOrWhiteSpace(tipoLancamentoFiltro))
        {
            query = query.Where(x => x.TipoLancamento == tipoLancamentoFiltro);
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

        if (!string.IsNullOrWhiteSpace(empresaSelecionada))
        {
            var empresa = empresaSelecionada.Trim();
            query = query.Where(x => x.EmpresaFrequente != null && x.EmpresaFrequente.Nome.Contains(empresa));
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
        tipoLancamentoFiltro = null;
        categoriaId = null;
        formaPagamentoId = null;
        statusId = null;
        empresaSelecionada = null;
        dataInicio = null;
        dataFim = null;
        ordenarPor = "vencimento";
        direcaoOrdenacao = "asc";
        await BuscarAsync();
    }

    private void Editar(MovimentacaoFinanceira item)
    {
        editandoId = item.Id;
        editandoTipoLancamento = NormalizarTipoLancamento(item.TipoLancamento);
        editandoDescricao = item.Descricao;
        editandoEmpresaNome = item.EmpresaFrequente?.Nome ?? string.Empty;
        editandoValor = item.Valor;
        editandoDataVencimento = item.DataVencimento;
        editandoDataPagamento = item.DataPagamento;
        editandoCategoriaId = item.CategoriaGastoId;
        editandoFormaPagamentoId = item.FormaPagamentoId;
        editandoStatusId = item.StatusPendenciaId;
    }

    private async Task SalvarAsync()
    {
        if (editandoId is null || string.IsNullOrWhiteSpace(editandoDescricao))
        {
            return;
        }

        var item = await DbContext.MovimentacoesFinanceiras
            .Include(x => x.StatusPendencia)
            .FirstOrDefaultAsync(x => x.Id == editandoId.Value && x.ApplicationUserId == applicationUserId);

        if (item is null)
        {
            return;
        }

        var statusAnteriorPago = string.Equals(item.StatusPendencia?.Nome, "Pago", StringComparison.OrdinalIgnoreCase);
        var statusNovoNome = statusPendencias.FirstOrDefault(x => x.Id == editandoStatusId)?.Nome;
        var statusNovoPago = string.Equals(statusNovoNome, "Pago", StringComparison.OrdinalIgnoreCase);

        item.TipoLancamento = NormalizarTipoLancamento(editandoTipoLancamento);
        item.Descricao = editandoDescricao.Trim();
        item.EmpresaFrequenteId = await ObterOuCriarEmpresaAsync(editandoEmpresaNome);
        item.Valor = editandoValor;
        item.DataVencimento = editandoDataVencimento;
        if (!statusAnteriorPago && statusNovoPago)
        {
            item.DataPagamento = DateTime.Today;
        }
        item.CategoriaGastoId = editandoCategoriaId;
        item.FormaPagamentoId = editandoFormaPagamentoId;
        item.StatusPendenciaId = editandoStatusId;

        await DbContext.SaveChangesAsync();
        await CartaoCreditoLimiteHelper.RecalcularLimitesEmUsoAsync(DbContext, applicationUserId);
        Cancelar();
        await BuscarAsync();
    }

    private void Cancelar()
    {
        editandoId = null;
        editandoTipoLancamento = "Pagar";
        editandoDescricao = string.Empty;
        editandoEmpresaNome = string.Empty;
        editandoValor = 0m;
        editandoDataVencimento = default;
        editandoDataPagamento = null;
        editandoCategoriaId = default;
        editandoFormaPagamentoId = default;
        editandoStatusId = default;
    }

    private async Task RemoverAsync(Guid id)
    {
        var item = await DbContext.MovimentacoesFinanceiras
            .FirstOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == applicationUserId);

        if (item is null)
        {
            return;
        }

        DbContext.MovimentacoesFinanceiras.Remove(item);
        await DbContext.SaveChangesAsync();
        await CartaoCreditoLimiteHelper.RecalcularLimitesEmUsoAsync(DbContext, applicationUserId);
        await TourProgressService.RegistrarAcaoAsync(TourAction.LancamentoExcluido);
        var usuarioTour = await TourProgressService.ObterUsuarioAsync();
        if (usuarioTour?.TourAtivo == true && usuarioTour.TourEtapa >= TourProgressService.EtapaFinal)
        {
            NavigationManager.NavigateTo("primeiro-tour");
            return;
        }

        if (editandoId == id)
        {
            Cancelar();
        }

        await BuscarAsync();
    }

    private async Task<Guid?> ObterOuCriarEmpresaAsync(string? nomeEmpresa)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            return null;
        }

        var nome = NormalizarEspacos(nomeEmpresa ?? string.Empty);
        if (string.IsNullOrWhiteSpace(nome))
        {
            return null;
        }

        var chave = NormalizarNomeEmpresa(nome);
        var empresas = await DbContext.EmpresasFrequentes
            .Where(x => x.ApplicationUserId == applicationUserId)
            .ToListAsync();
        var existente = empresas.FirstOrDefault(x => NormalizarNomeEmpresa(x.Nome) == chave);

        if (existente is not null)
        {
            return existente.Id;
        }

        var nova = new EmpresaFrequente
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = applicationUserId,
            Nome = nome
        };

        DbContext.EmpresasFrequentes.Add(nova);
        try
        {
            await DbContext.SaveChangesAsync();
            empresasFrequentes = await DbContext.EmpresasFrequentes
                .Where(x => x.ApplicationUserId == applicationUserId)
                .OrderBy(x => x.Nome)
                .ToListAsync();
            return nova.Id;
        }
        catch (DbUpdateException)
        {
            // Colisao de nome por concorrencia/normalizacao: reaproveita registro existente.
            DbContext.Entry(nova).State = EntityState.Detached;
            var concorrente = (await DbContext.EmpresasFrequentes
                .Where(x => x.ApplicationUserId == applicationUserId)
                .ToListAsync())
                .FirstOrDefault(x => NormalizarNomeEmpresa(x.Nome) == chave);
            return concorrente?.Id;
        }
    }

    private async Task NormalizarTiposLancamentoAsync()
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            return;
        }

        var itens = await DbContext.MovimentacoesFinanceiras
            .Where(x => x.ApplicationUserId == applicationUserId)
            .Where(x => x.TipoLancamento != "Pagar" && x.TipoLancamento != "Receber")
            .ToListAsync();

        if (itens.Count == 0)
        {
            return;
        }

        foreach (var item in itens)
        {
            item.TipoLancamento = "Pagar";
        }

        await DbContext.SaveChangesAsync();
    }

    private static string NormalizarTipoLancamento(string? tipo) =>
        string.Equals(tipo, "Receber", StringComparison.OrdinalIgnoreCase) ? "Receber" : "Pagar";

    private static string NormalizarNomeEmpresa(string valor)
    {
        var normalizado = NormalizarEspacos(valor).Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);

        foreach (var ch in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }

    private static string NormalizarEspacos(string valor) =>
        string.Join(' ', (valor ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));

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
