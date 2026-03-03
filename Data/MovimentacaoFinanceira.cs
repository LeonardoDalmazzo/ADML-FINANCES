using System.ComponentModel.DataAnnotations;

namespace ADML_FINANCES.Data;

public class MovimentacaoFinanceira
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string TipoLancamento { get; set; } = "Pagar";

    [Required]
    [MaxLength(150)]
    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime DataLancamento { get; set; }

    public DateTime DataVencimento { get; set; }

    public DateTime? DataPagamento { get; set; }

    [Required]
    [MaxLength(120)]
    public string Usuario { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Observacao { get; set; }

    public Guid? EmpresaFrequenteId { get; set; }
    public EmpresaFrequente? EmpresaFrequente { get; set; }

    public Guid CategoriaGastoId { get; set; }
    public CategoriaGasto? CategoriaGasto { get; set; }

    public Guid FormaPagamentoId { get; set; }
    public FormaPagamento? FormaPagamento { get; set; }

    public Guid StatusPendenciaId { get; set; }
    public StatusPendencia? StatusPendencia { get; set; }
}
