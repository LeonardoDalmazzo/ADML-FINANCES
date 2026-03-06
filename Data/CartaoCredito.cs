using System.ComponentModel.DataAnnotations;

namespace ADML_FINANCES.Data;

public class CartaoCredito
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string ApplicationUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Banco { get; set; } = string.Empty;

    public decimal LimiteDisponivel { get; set; }

    public decimal LimiteEmUso { get; set; }

    [Range(1, 31)]
    public int DiaFechamento { get; set; }

    [Range(1, 31)]
    public int DiaVencimento { get; set; }

    public Guid FormaPagamentoId { get; set; }
    public FormaPagamento? FormaPagamento { get; set; }
}
