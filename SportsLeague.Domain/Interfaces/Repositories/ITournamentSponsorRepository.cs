using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
{
    // Método solicitado para listar sponsors de un torneo específico
    Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId);

    // Método inferido: Listar torneos asociados a un sponsor
    Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);

    // Método inferido: Verificar si ya existe la relación antes de crearla
    Task<bool> IsSponsorAlreadyInTournamentAsync(int tournamentId, int sponsorId);


}