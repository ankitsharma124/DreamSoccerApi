

using DreamSoccer.Core.Entities;
using System.Collections.Generic;

namespace DreamSoccer.Core.Dtos.Teams
{
    public class TeamInformationDto:BaseEntityDto
    {
        public string TeamName { get; set; }
        public string Country { get; set; }
        public long Budget { get; set; }
        public long TeamValue { get; set; }

        public virtual ICollection<PlayersInformationDto> Players { get; set; } = new List<PlayersInformationDto>();
    }
}
