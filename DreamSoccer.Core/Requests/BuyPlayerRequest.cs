using System.ComponentModel.DataAnnotations;

namespace DreamSoccer.Core.Dtos.User
{
    public class BuyPlayerRequest
    {
        [Required]
        public int TransferId { get; set; }
        public int TeamId { get; set; }

    }
}