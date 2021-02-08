using DreamSoccer.Core.Dtos;

namespace DreamSoccer.Core.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public RoleEnum Role { get; set; } = RoleEnum.Team_Owner;
        public Team Team { get; set; }
        public int? TeamId { get; set; }
    }
}