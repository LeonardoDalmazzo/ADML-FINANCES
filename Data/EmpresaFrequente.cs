using System.ComponentModel.DataAnnotations;

namespace ADML_FINANCES.Data;

public class EmpresaFrequente
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string ApplicationUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;
}
