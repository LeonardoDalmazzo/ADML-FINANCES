using ADML_FINANCES.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ADML_FINANCES.Components.Pages;

public partial class Visualizacao : ComponentBase
{
    [Inject] private ApplicationDbContext DbContext { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private readonly List<string> tiposLancamento = ["Pagar", "Receber"];
    private List<MovimentacaoFinanceira> movimentacoes = [];
    private List<EmpresaFrequente> empresasFrequentes = [];
    private List<CategoriaGasto> categorias = [];
    private List<FormaPagamento> formasPagamento = [];
    private List<StatusPendencia> statusPendencias = [];

    private string? usuarioAtual;
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

    private bool cadastroEmLote;
    private string novoTipoLancamento = "Pagar";
    private string novoDescricao = string.Empty;
    private string novoEmpresaNome = string.Empty;
    private string novoObservacao = string.Empty;
    private decimal novoValor;
    private Guid novoCategoriaId;
    private Guid novoFormaPagamentoId;
    private Guid novoStatusId;
    private DateTime novoDataLancamento = DateTime.Today;
    private DateTime novoDataVencimento = DateTime.Today;
    private DateTime? novoDataPagamento;
    private int novoQuantidadeLote = 1;
    private DateTime novoMesInicial = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateTime? novoMesFinal;
    private DateTime? novoDataPagamentoBase;
    private string? mensagemCadastro;

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

        await NormalizarTiposLancamentoAsync();

        empresasFrequentes = await DbContext.EmpresasFrequentes.OrderBy(x => x.Nome).ToListAsync();
        categorias = await DbContext.CategoriasGasto.OrderBy(x => x.Nome).ToListAsync();
        formasPagamento = await DbContext.FormasPagamento.OrderBy(x => x.Nome).ToListAsync();
        statusPendencias = await DbContext.StatusPendencias.OrderBy(x => x.Nome).ToListAsync();

        if (categorias.Count > 0)
        {
            novoCategoriaId = categorias[0].Id;
        }

        if (formasPagamento.Count > 0)
        {
            novoFormaPagamentoId = formasPagamento[0].Id;
        }

        if (statusPendencias.Count > 0)
        {
            novoStatusId = statusPendencias[0].Id;
        }

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

        if (!string.IsNullOrWhiteSpace(usuarioAtual))
        {
            query = query.Where(x => x.Usuario == usuarioAtual);
        }

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

    private async Task CadastrarAsync()
    {
        mensagemCadastro = null;
        if (string.IsNullOrWhiteSpace(usuarioAtual))
        {
            mensagemCadastro = "Usuario nao autenticado.";
            return;
        }

        if (string.IsNullOrWhiteSpace(novoDescricao) || novoValor <= 0)
        {
            mensagemCadastro = "Preencha tipo, descricao e valor valido.";
            return;
        }

        var empresaId = await ObterOuCriarEmpresaAsync(novoEmpresaNome);
        var descricao = novoDescricao.Trim();
        var observacao = string.IsNullOrWhiteSpace(novoObservacao) ? null : novoObservacao.Trim();

        var tipoNormalizado = NormalizarTipoLancamento(novoTipoLancamento);

        if (cadastroEmLote)
        {
            var totalParcelas = CalcularQuantidadeParcelas();
            if (totalParcelas <= 0)
            {
                mensagemCadastro = "Informe quantidade maior que zero ou mes final valido.";
                return;
            }

            var baseVencimento = new DateTime(novoMesInicial.Year, novoMesInicial.Month, Math.Min(novoDataVencimento.Day, DateTime.DaysInMonth(novoMesInicial.Year, novoMesInicial.Month)));

            for (var i = 0; i < totalParcelas; i++)
            {
                var vencimento = AjustarDiaMes(baseVencimento.AddMonths(i), novoDataVencimento.Day);
                DateTime? pagamento = null;
                if (novoDataPagamentoBase.HasValue)
                {
                    pagamento = AjustarDiaMes(vencimento, novoDataPagamentoBase.Value.Day);
                }

                DbContext.MovimentacoesFinanceiras.Add(new MovimentacaoFinanceira
                {
                    Id = Guid.NewGuid(),
                    TipoLancamento = tipoNormalizado,
                    Descricao = descricao,
                    Valor = novoValor,
                    DataLancamento = AjustarDiaMes(vencimento, novoDataLancamento.Day),
                    DataVencimento = vencimento,
                    DataPagamento = pagamento,
                    Usuario = usuarioAtual,
                    Observacao = observacao,
                    EmpresaFrequenteId = empresaId,
                    CategoriaGastoId = novoCategoriaId,
                    FormaPagamentoId = novoFormaPagamentoId,
                    StatusPendenciaId = novoStatusId
                });
            }

            await DbContext.SaveChangesAsync();
            mensagemCadastro = $"{totalParcelas} lancamentos cadastrados em lote.";
        }
        else
        {
            DbContext.MovimentacoesFinanceiras.Add(new MovimentacaoFinanceira
            {
                Id = Guid.NewGuid(),
                TipoLancamento = tipoNormalizado,
                Descricao = descricao,
                Valor = novoValor,
                DataLancamento = novoDataLancamento,
                DataVencimento = novoDataVencimento,
                DataPagamento = novoDataPagamento,
                Usuario = usuarioAtual,
                Observacao = observacao,
                EmpresaFrequenteId = empresaId,
                CategoriaGastoId = novoCategoriaId,
                FormaPagamentoId = novoFormaPagamentoId,
                StatusPendenciaId = novoStatusId
            });

            await DbContext.SaveChangesAsync();
            mensagemCadastro = "Lancamento cadastrado.";
        }

        LimparFormulario();
        await BuscarAsync();
    }

    private void LimparFormulario()
    {
        novoTipoLancamento = "Pagar";
        novoDescricao = string.Empty;
        novoEmpresaNome = string.Empty;
        novoObservacao = string.Empty;
        novoValor = 0m;
        novoDataLancamento = DateTime.Today;
        novoDataVencimento = DateTime.Today;
        novoDataPagamento = null;
        novoQuantidadeLote = 1;
        novoMesInicial = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        novoMesFinal = null;
        novoDataPagamentoBase = null;
    }

    private int CalcularQuantidadeParcelas()
    {
        if (novoQuantidadeLote > 0)
        {
            return novoQuantidadeLote;
        }

        if (!novoMesFinal.HasValue)
        {
            return 0;
        }

        var inicio = new DateTime(novoMesInicial.Year, novoMesInicial.Month, 1);
        var fim = new DateTime(novoMesFinal.Value.Year, novoMesFinal.Value.Month, 1);
        if (fim < inicio)
        {
            return 0;
        }

        return ((fim.Year - inicio.Year) * 12) + fim.Month - inicio.Month + 1;
    }

    private static DateTime AjustarDiaMes(DateTime referencia, int dia)
    {
        var diaFinal = Math.Min(Math.Max(dia, 1), DateTime.DaysInMonth(referencia.Year, referencia.Month));
        return new DateTime(referencia.Year, referencia.Month, diaFinal);
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
            .FirstOrDefaultAsync(x => x.Id == editandoId.Value && x.Usuario == usuarioAtual);

        if (item is null)
        {
            return;
        }

        item.TipoLancamento = NormalizarTipoLancamento(editandoTipoLancamento);
        item.Descricao = editandoDescricao.Trim();
        item.EmpresaFrequenteId = await ObterOuCriarEmpresaAsync(editandoEmpresaNome);
        item.Valor = editandoValor;
        item.DataVencimento = editandoDataVencimento;
        item.DataPagamento = editandoDataPagamento;
        item.CategoriaGastoId = editandoCategoriaId;
        item.FormaPagamentoId = editandoFormaPagamentoId;
        item.StatusPendenciaId = editandoStatusId;

        await DbContext.SaveChangesAsync();
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
            .FirstOrDefaultAsync(x => x.Id == id && x.Usuario == usuarioAtual);

        if (item is null)
        {
            return;
        }

        DbContext.MovimentacoesFinanceiras.Remove(item);
        await DbContext.SaveChangesAsync();

        if (editandoId == id)
        {
            Cancelar();
        }

        await BuscarAsync();
    }

    private async Task<Guid?> ObterOuCriarEmpresaAsync(string? nomeEmpresa)
    {
        var nome = (nomeEmpresa ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nome))
        {
            return null;
        }

        var existente = await DbContext.EmpresasFrequentes
            .FirstOrDefaultAsync(x => x.Nome.ToUpper() == nome.ToUpper());

        if (existente is not null)
        {
            return existente.Id;
        }

        var nova = new EmpresaFrequente
        {
            Id = Guid.NewGuid(),
            Nome = nome
        };

        DbContext.EmpresasFrequentes.Add(nova);
        await DbContext.SaveChangesAsync();

        empresasFrequentes = await DbContext.EmpresasFrequentes.OrderBy(x => x.Nome).ToListAsync();
        return nova.Id;
    }

    private async Task NormalizarTiposLancamentoAsync()
    {
        if (string.IsNullOrWhiteSpace(usuarioAtual))
        {
            return;
        }

        var itens = await DbContext.MovimentacoesFinanceiras
            .Where(x => x.Usuario == usuarioAtual)
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
