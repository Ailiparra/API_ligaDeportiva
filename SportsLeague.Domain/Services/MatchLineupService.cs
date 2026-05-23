using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly MatchValidationHelper _validationHelper;

    public MatchLineupService(
        IMatchLineupRepository matchLineupRepository,
        IMatchRepository matchRepository,
        MatchValidationHelper validationHelper)
    {
        _matchLineupRepository = matchLineupRepository;
        _matchRepository = matchRepository;
        _validationHelper = validationHelper;
    }

    public async Task<MatchLineup> AddPlayerToLineupAsync(int matchId, MatchLineup lineup)
    {
        // V1: El partido debe existir
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");
        }

        // V6: El partido debe estar en estado Scheduled
        if (match.Status != MatchStatus.Scheduled)
        {
            throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");
        }

        // V2 & V3: El jugador debe existir y pertenecer a uno de los equipos del partido
        // ValidatePlayerInMatchAsync realiza exactamente estas dos validaciones:
        // - Lanza KeyNotFoundException si el jugador no existe.
        // - Lanza InvalidOperationException si el jugador no pertenece a ninguno de los equipos del partido.
        var player = await _validationHelper.ValidatePlayerInMatchAsync(lineup.PlayerId, match);

        // V4: El jugador no puede estar registrado dos veces en la misma alineación del partido
        var alreadyRegistered = await _matchLineupRepository.ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (alreadyRegistered)
        {
            throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");
        }

        // V5: Máximo 11 titulares por equipo por partido
        if (lineup.IsStarter)
        {
            var teamLineup = await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, player.TeamId);
            int startersCount = teamLineup.Count(ml => ml.IsStarter);
            if (startersCount >= 11)
            {
                throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
            }
        }

        lineup.MatchId = matchId;
        return await _matchLineupRepository.CreateAsync(lineup);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");
        }
        return await _matchLineupRepository.GetByMatchAsync(matchId);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAndTeamAsync(int matchId, int teamId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");
        }
        return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    public async Task RemovePlayerFromLineupAsync(int matchId, int id)
    {
        var lineup = await _matchLineupRepository.GetByIdAsync(id);
        if (lineup == null)
        {
            throw new KeyNotFoundException($"No se encontró la alineación con ID {id}");
        }
        if (lineup.MatchId != matchId)
        {
            throw new InvalidOperationException("La alineación no corresponde a este partido");
        }
        await _matchLineupRepository.DeleteAsync(id);
    }
}
