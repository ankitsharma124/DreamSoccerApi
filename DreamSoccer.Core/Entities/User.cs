using System;

namespace DreamSoccer.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool DelFlag { get; set; } = false;
        public Role Role { get; set; } = Role.Team_Owner;
    }
}