using SportsLeague.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System;

namespace SportsLeague.Domain.Entities;

public class Sponsor : AuditBase
{

    public string Name { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? WebsiteUrl { get; set; }

    public SponsorCategory Category { get; set; }


    // Navigation Property para la relación N:M
    public ICollection<TournamentSponsor> TournamentSponsors { get; set; } = new List<TournamentSponsor>();


}