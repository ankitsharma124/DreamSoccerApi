using DreamSoccer.Core.Dtos;
using DreamSoccer.Core.Entities.Enums;

namespace DreamSoccer.Core.Entities
{
    public class PlayersInformationDto : BaseEntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public long Value { get; set; }
        public PositionEnum Position { get; set; }
    }
}