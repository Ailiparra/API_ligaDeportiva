using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public SponsorService(ISponsorRepository sponsorRepository, ITournamentSponsorRepository tournamentSponsorRepository,
        ITournamentRepository tournamentRepository)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task LinkToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
    {
        // 1. Validación: ContractAmount > 0
        if (contractAmount <= 0)
            throw new ArgumentException("El monto del contrato debe ser mayor a 0.");

        // 2. Validación: ¿Existe el Sponsor?
        var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
        if (sponsor == null)
            throw new KeyNotFoundException("El patrocinador no existe.");

        // 3. Validación: ¿Existe el Torneo?
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new KeyNotFoundException("El torneo no existe.");

        // 4. Validación: ¿Ya está vinculado? (Evitar duplicados)
        if (await _tournamentSponsorRepository.IsSponsorAlreadyInTournamentAsync(tournamentId, sponsorId))
            throw new ArgumentException("Este patrocinador ya está vinculado a este torneo.");

        // Crear la vinculación
        var tournamentSponsor = new TournamentSponsor
        {
            SponsorId = sponsorId,
            TournamentId = tournamentId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow
        };

        await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
    }

    public async Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId)
    {
        // Buscar la relación específica
        var allSponsors = await _tournamentSponsorRepository.GetByTournamentIdAsync(tournamentId);
        var relation = allSponsors.FirstOrDefault(ts => ts.SponsorId == sponsorId);

        if (relation == null)
            throw new KeyNotFoundException("No existe una vinculación entre este patrocinador y el torneo.");

        await _tournamentSponsorRepository.DeleteAsync(relation.Id);
    }

    public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        return await _sponsorRepository.GetByIdAsync(id);
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        // Validación de nombre duplicado
        if (await _sponsorRepository.NameExistsAsync(sponsor.Name))
        {
            throw new ArgumentException($"Ya existe un patrocinador con el nombre '{sponsor.Name}'.");
        }

        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
        if (existingSponsor == null)
            throw new KeyNotFoundException("El patrocinador no existe.");

        // Validar nombre duplicado solo si el nombre cambió
        if (existingSponsor.Name != sponsor.Name &&
            await _sponsorRepository.NameExistsAsync(sponsor.Name))
        {
            throw new ArgumentException($"El nombre '{sponsor.Name}' ya está en uso.");
        }

        // Mapeo manual de campos (o podrías usar AutoMapper aquí)
        existingSponsor.Name = sponsor.Name;
        existingSponsor.ContactEmail = sponsor.ContactEmail;
        existingSponsor.Phone = sponsor.Phone;
        existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
        existingSponsor.Category = sponsor.Category;

        await _sponsorRepository.UpdateAsync(existingSponsor);
    }

    public async Task DeleteAsync(int id)
    {
        var sponsor = await _sponsorRepository.GetByIdAsync(id);
        if (sponsor == null)
            throw new KeyNotFoundException("No se encontró el patrocinador a eliminar.");

        await _sponsorRepository.DeleteAsync(id);
    }
}