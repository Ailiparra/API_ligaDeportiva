using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;

    public SponsorController(ISponsorService sponsorService, IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null) return NotFound();

        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO request)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(request);
            var createdSponsor = await _sponsorService.CreateAsync(sponsor);
            var response = _mapper.Map<SponsorResponseDTO>(createdSponsor);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SponsorRequestDTO request)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(request);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    // 6. POST: Vincular un sponsor a un torneo
    // Ruta: POST /api/Sponsor/{id}/tournaments
    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult<TournamentSponsorResponseDTO>> LinkToTournament(int id, [FromBody] TournamentSponsorRequestDTO request)
    {
        try
        {
            // Forzamos que el ID del Sponsor en la URL coincida con el del cuerpo del JSON
            if (id != request.SponsorId)
            {
                return BadRequest("El ID del patrocinador en la URL no coincide con el del cuerpo de la solicitud.");
            }

            await _sponsorService.LinkToTournamentAsync(request.SponsorId, request.TournamentId, request.ContractAmount);

            // Retornamos la relación creada (podemos consultar al repo para devolver el objeto completo con nombres)
            return Ok("Patrocinador vinculado exitosamente al torneo.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // 7. GET: Listar los torneos de un sponsor específico
    // Ruta: GET /api/Sponsor/{id}/tournaments
    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetSponsorTournaments(int id)
    {
        try
        {
            var relations = await _sponsorService.GetTournamentsBySponsorAsync(id);
            var response = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(relations);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // 8. DELETE: Desvincular un sponsor de un torneo
    // Ruta: DELETE /api/Sponsor/{id}/tournaments/{tournamentId}
    [HttpDelete("{id}/tournaments/{tournamentId}")]
    public async Task<IActionResult> UnlinkFromTournament(int id, int tournamentId)
    {
        try
        {
            await _sponsorService.UnlinkFromTournamentAsync(id, tournamentId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}