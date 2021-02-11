using DreamSoccer.Core.Dtos;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using System.Collections.Generic;

namespace DreamSoccer.Core.Dtos.Players
{
    public class TeamDto : BaseEntityDto
    {
        public string TeamName { get; set; }
        public string Country { get; set; }
        public long Budget { get; set; }
        public UserOwnerDto Owner { get; set; }

        public long TeamValue { get; set; }
    }
    public class TeamInput : TeamDto
    {
        public List<PlayerDto> Players { get; set; }

    }
}