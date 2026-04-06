using System.ComponentModel.DataAnnotations;

namespace SportsLeague.API.DTOs.Request;

public class SponsorRequestDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email de contacto es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [StringLength(150)]
    public string ContactEmail { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
    public string? Phone { get; set; }

    [Url(ErrorMessage = "El formato de la URL no es válido.")]
    [StringLength(200)]
    public string? WebsiteUrl { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public SponsorCategory Category { get; set; }
}