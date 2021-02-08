using DreamSoccer.Core.Dtos;
using DreamSoccer.Core.Dtos.User;

namespace DreamSoccer.Core.Dtos.Players
{
    public class TeamDto : BaseEntityDto
    {
        public string TeamName { get; set; }
        public string Country { get; set; }
        public long Budget { get; set; }
        public UserOwnerDto Owner { get; set; }
    }
}