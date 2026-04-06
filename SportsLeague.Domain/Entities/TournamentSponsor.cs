using System;
using System.Collections.Generic;
using System.Text;
using System;
namespace SportsLeague.Domain.Entities;

public class TournamentSponsor : AuditBase
{
    //identificadores de las FK
    public int TournamentId { get; set; }
    public int SponsorId { get; set; }

    // Propiedades adicionales de la tabla
    public decimal ContractAmount { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties (Configuración de FKs)
    public Tournament Tournament { get; set; } = null!;
    public Sponsor Sponsor { get; set; } = null!;
}