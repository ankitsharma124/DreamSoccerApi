using AutoMapper;
using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Dtos.Teams;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Requests;
using DreamSoccer.Core.Responses;
using System.Linq;

namespace DreamSoccer.Core.Configurations
{
    public class ValueTeamResolver : IValueResolver<Team, TeamDto, long>
    {
        public long Resolve(Team source, TeamDto destination, long member, ResolutionContext context)
        {
            return source.Players.Sum(n => n.Value);
        }
    }
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserOwnerDto>().ReverseMap();
            CreateMap<Player, PlayerDto>().ReverseMap();
            CreateMap<Player, PlayersInformationDto>().ReverseMap();
            CreateMap<Team, TeamDto>()
            .ReverseMap();
            CreateMap<Team, TeamDto>().ForMember(dest => dest.TeamValue, opt => opt.MapFrom(m => m.Players.Sum(i => i.Value)));

            CreateMap<Team, TeamInformationDto>()
                .ForMember(dest => dest.TeamValue, opt => opt.MapFrom(m => m.Players.Sum(i => i.Value)))
            .ReverseMap();
            CreateMap<SearchPlayerRequest, SearchPlayerFilter>().ReverseMap();
            CreateMap<BuyPlayerResult, BuyPlayerResultResponse>().ReverseMap();
            CreateMap<TransferList, SearchResultDto>().ReverseMap();

            CreateMap<SearchTeamRequest, SearchTeamFilter>().ReverseMap();

        }
    }
}
