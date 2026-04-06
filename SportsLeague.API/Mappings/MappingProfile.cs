using AutoMapper;

using SportsLeague.API.DTOs.Request;

using SportsLeague.API.DTOs.Response;

using SportsLeague.Domain.Entities;


namespace SportsLeague.API.Mappings;


public class MappingProfile : Profile

{

    public MappingProfile()

    {

        // Team mappings

        CreateMap<TeamRequestDTO, Team>();

        CreateMap<Team, TeamResponseDTO>();


        // Player mappings

        CreateMap<PlayerRequestDTO, Player>();

        CreateMap<Player, PlayerResponseDTO>()

            .ForMember(

                dest => dest.TeamName,

                opt => opt.MapFrom(src => src.Team.Name));


        // Referee mappings

        CreateMap<RefereeRequestDTO, Referee>();

        CreateMap<Referee, RefereeResponseDTO>();


        // Tournament mappings

        CreateMap<TournamentRequestDTO, Tournament>();

        CreateMap<Tournament, TournamentResponseDTO>()

            .ForMember(

                dest => dest.TeamsCount,

                opt => opt.MapFrom(src =>

                    src.TournamentTeams != null ? src.TournamentTeams.Count : 0));

        // ── Sponsor mappings ──
        CreateMap<Sponsor, SponsorResponseDTO>();

        CreateMap<SponsorRequestDTO, Sponsor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TournamentSponsors, opt => opt.Ignore());

        // ── TournamentSponsor mappings (N:M) ──
        CreateMap<TournamentSponsor, TournamentSponsorResponseDTO>()
            .ForMember(dest => dest.SponsorName,
                opt => opt.MapFrom(src => src.Sponsor != null ? src.Sponsor.Name : string.Empty))
            .ForMember(dest => dest.TournamentName,
                opt => opt.MapFrom(src => src.Tournament != null ? src.Tournament.Name : string.Empty));

        CreateMap<TournamentSponsorRequestDTO, TournamentSponsor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tournament, opt => opt.Ignore())
            .ForMember(dest => dest.Sponsor, opt => opt.Ignore());

    }

}