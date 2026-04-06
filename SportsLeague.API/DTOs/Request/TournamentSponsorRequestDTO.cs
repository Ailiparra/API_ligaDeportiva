using System.ComponentModel.DataAnnotations;

namespace SportsLeague.API.DTOs.Request;

public class TournamentSponsorRequestDTO
{
    [Required(ErrorMessage = "El ID del torneo es obligatorio.")]
    public int TournamentId { get; set; }

    [Required(ErrorMessage = "El ID del patrocinador es obligatorio.")]
    public int SponsorId { get; set; }

    [Required(ErrorMessage = "El monto del contrato es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto del contrato debe ser mayor a 0.")]
    public decimal ContractAmount { get; set; }

    [Required(ErrorMessage = "La fecha de vinculación es obligatoria.")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
