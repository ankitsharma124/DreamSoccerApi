using DreamSoccer.Core.Entities.Enums;

namespace DreamSoccer.Core.Dtos.User
{
    public class PlayerReqeust
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public long Value { get; set; }
        public int? TeamId { get; set; }
        public PositionEnum Position { get; set; }
    }

}