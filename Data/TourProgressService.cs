using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ADML_FINANCES.Data;

public enum TourAction
{
    CartaoCadastrado,
    EmpresaCadastrada,
    StatusCadastrado,
    CategoriaCadastrada,
    FormaPagamentoCadastrada,
    LancamentoCadastrado,
    LancamentoExcluido
}

public sealed class TourProgressService(
    AuthenticationStateProvider authenticationStateProvider,
    UserManager<ApplicationUser> userManager)
{
    public const int EtapaFinal = 16;

    public async Task<ApplicationUser?> ObterUsuarioAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return await userManager.GetUserAsync(authState.User);
    }

    public async Task IniciarTourAsync()
    {
        var usuario = await ObterUsuarioAsync();
        if (usuario is null)
        {
            return;
        }

        usuario.TourAtivo = true;
        usuario.TourEtapa = 0;
        usuario.OnboardingConcluido = false;
        await userManager.UpdateAsync(usuario);
    }

    public async Task PularOuFinalizarTourAsync()
    {
        var usuario = await ObterUsuarioAsync();
        if (usuario is null)
        {
            return;
        }

        usuario.TourAtivo = false;
        usuario.TourEtapa = 0;
        usuario.OnboardingConcluido = true;
        await userManager.UpdateAsync(usuario);
    }

    public async Task AvancarEtapaAsync()
    {
        var usuario = await ObterUsuarioAsync();
        if (usuario is null || !usuario.TourAtivo)
        {
            return;
        }

        if (!EtapaConcluida(usuario.TourEtapa))
        {
            return;
        }

        if (usuario.TourEtapa >= EtapaFinal)
        {
            return;
        }

        usuario.TourEtapa++;
        await userManager.UpdateAsync(usuario);
    }

    public async Task RegistrarAcaoAsync(TourAction acao)
    {
        var usuario = await ObterUsuarioAsync();
        if (usuario is null || !usuario.TourAtivo)
        {
            return;
        }

        var etapaAtual = usuario.TourEtapa;
        var etapaConcluida = etapaAtual switch
        {
            0 => acao == TourAction.CartaoCadastrado,
            2 => acao == TourAction.FormaPagamentoCadastrada,
            4 => acao == TourAction.CategoriaCadastrada,
            6 => acao == TourAction.StatusCadastrado,
            8 => acao == TourAction.EmpresaCadastrada,
            10 => acao == TourAction.LancamentoCadastrado,
            14 => acao == TourAction.LancamentoExcluido,
            _ => false
        };

        if (!etapaConcluida)
        {
            return;
        }

        if (usuario.TourEtapa % 2 == 0)
        {
            usuario.TourEtapa++;
            await userManager.UpdateAsync(usuario);
        }
    }

    public async Task RegistrarNavegacaoAsync(string? path)
    {
        var usuario = await ObterUsuarioAsync();
        if (usuario is null || !usuario.TourAtivo || string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var rota = path.Trim();
        var etapaAtual = usuario.TourEtapa;

        if (etapaAtual == 12 && rota.StartsWith("dashboard", StringComparison.OrdinalIgnoreCase))
        {
            usuario.TourEtapa = 13;
            await userManager.UpdateAsync(usuario);
        }
    }

    public static TourEtapaInfo ObterEtapaInfo(int etapa) => etapa switch
    {
        0 or 1 => new(
            "1/8",
            "Cadastre um cartao",
            "Abra Cartoes e cadastre 1 cartao.",
            "Ao criar cartao, ele tambem entra em Formas de Pagamento.",
            ""),
        2 or 3 => new(
            "2/8",
            "Cadastre forma de pagamento",
            "Abra Formas de Pagamento e cadastre 1 item.",
            "Pagina de gestao: use Acoes para editar ou remover.",
            ""),
        4 or 5 => new(
            "3/8",
            "Cadastre uma categoria",
            "Abra Categorias de gasto e cadastre 1 item.",
            "Pagina de gestao: use Acoes para editar ou remover.",
            ""),
        6 or 7 => new(
            "4/8",
            "Cadastre um status",
            "Abra Status de pendencias e cadastre 1 item.",
            "Pagina de gestao: use Acoes para editar ou remover.",
            ""),
        8 or 9 => new(
            "5/8",
            "Cadastre uma empresa",
            "Abra Empresas frequentes e cadastre 1 item.",
            "Pagina de gestao: use Acoes para editar ou remover.",
            ""),
        10 or 11 => new(
            "6/8",
            "Cadastre um lancamento",
            "Abra Lancamentos e cadastre 1 item.",
            "Use categorias, status, formas e cartoes cadastrados nas etapas anteriores.",
            ""),
        12 or 13 => new(
            "7/8",
            "Visualize o dashboard",
            "Abra Dashboard para ver os indicadores.",
            "Acompanhe totais e vencimentos em um resumo rapido.",
            ""),
        14 or 15 => new(
            "8/8",
            "Exclua o lancamento na visualizacao",
            "Abra Visualizacao e apague 1 lancamento.",
            "Na coluna Acoes, voce pode editar ou remover itens.",
            ""),
        EtapaFinal => new(
            "Concluido",
            "Primeiros passos finalizados",
            "Tutorial concluido com sucesso.",
            "Voce viu o fluxo completo: gestao, lancamentos, dashboard e visualizacao.",
            ""),
        _ => new(
            "Concluido",
            "Primeiros passos finalizados",
            "Tutorial concluido.",
            "Reinicie quando quiser pelo menu lateral.",
            "")
    };

    public static bool EtapaConcluida(int etapa) => etapa % 2 == 1 || etapa >= EtapaFinal;

    public static string ObterRotaEtapa(int etapa) => etapa switch
    {
        0 or 1 => "gerenciar/cartoes",
        2 or 3 => "gerenciar/formas-pagamento",
        4 or 5 => "gerenciar/categorias-gasto",
        6 or 7 => "gerenciar/status-pendencias",
        8 or 9 => "gerenciar/empresas",
        10 or 11 => "gerenciar/lancamentos",
        12 or 13 => "dashboard",
        14 or 15 => "visualizacao",
        _ => "primeiro-tour"
    };

    public sealed record TourEtapaInfo(
        string Progresso,
        string Titulo,
        string Descricao,
        string Contexto,
        string OQueMaisPodeFazer);
}
