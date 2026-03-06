using System.ComponentModel.DataAnnotations;

namespace ADML_FINANCES.Data;

public class StatusPendencia
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string ApplicationUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(7)]
    public string Cor { get; set; } = "#000000";
}
