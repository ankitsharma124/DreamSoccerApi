using System.Collections.Generic;

namespace DreamSoccer.Core.Entities
{
    public class Team : BaseEntity
    {
        public string TeamName { get; set; }
        public string Country { get; set; }
        public long Budget { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public User Owner { get; set; }
    }
}