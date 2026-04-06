using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Repositories;

public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
{
    public TournamentSponsorRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId)
    {
        return await _dbSet
            .Include(ts => ts.Sponsor) // Carga los datos del Patrocinador
            .Where(ts => ts.TournamentId == tournamentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId)
    {
        return await _dbSet
            .Include(ts => ts.Tournament) // Carga los datos del Torneo
            .Where(ts => ts.SponsorId == sponsorId)
            .ToListAsync();
    }

    public async Task<bool> IsSponsorAlreadyInTournamentAsync(int tournamentId, int sponsorId)
    {
        return await _dbSet.AnyAsync(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId);
    }
}