using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Responses
{
    public class BuyPlayerResultResponse
    {
        public TeamDto PreviousTeam { get; set; }
        public TeamDto NextTeam { get; set; }
        public PlayerDto Player { get; set; }
    }
}