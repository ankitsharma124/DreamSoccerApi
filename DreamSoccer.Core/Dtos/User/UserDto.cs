using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Dtos.User
{
    public class UserDto : BaseEntity
    {
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public RoleEnum Role { get; set; } = RoleEnum.Team_Owner;
    }
}