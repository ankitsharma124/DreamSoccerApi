

using DreamSoccer.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DreamSoccer.Core.Dtos.Teams
{
    public class TeamInformationDto : BaseEntityDto
    {
        public string TeamName { get; set; }
        public string Country { get; set; }
        public long Budget { get; set; }
        public long TeamValue { get; set; }

        private ICollection<PlayersInformationDto> _players;

        public ICollection<PlayersInformationDto> Players
        {
            get
            {
                return _players.Where(n => !n.DelFlag).ToList();
            }
            set { _players = value; }
        }

    }
}
