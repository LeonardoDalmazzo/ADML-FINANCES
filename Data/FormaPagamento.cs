using System.ComponentModel.DataAnnotations;

namespace ADML_FINANCES.Data;

public class FormaPagamento
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string ApplicationUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [MaxLength(7)]
    public string Cor { get; set; } = "#0d6efd";

    public bool Situacao { get; set; } = true;
}
