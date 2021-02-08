using DreamSoccer.Core.Dtos;
using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Entities.Enums;

namespace DreamSoccer.Core.Entities
{
    public class PlayerDto : BaseEntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public long Value { get; set; }
        public TeamDto Team { get; set; }
        public int TeamId { get; set; }
        public PositionEnum Position { get; set; }
    }
}