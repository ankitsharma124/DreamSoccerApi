using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Responses
{
    public class BuyPlayerResultResponse
    {
        public TeamDto Team { get; set; }
        public PlayerDto Player { get; set; }
    }
}