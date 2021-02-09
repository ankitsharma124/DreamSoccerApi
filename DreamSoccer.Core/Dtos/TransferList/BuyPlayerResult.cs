using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Dtos.TransferList
{
    public class BuyPlayerResult
    {
        public Team PreviousTeam { get; set; }
        public Team NextTeam { get; set; }
        public Player Player { get; set; }
    }
}
