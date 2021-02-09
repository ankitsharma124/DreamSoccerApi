using System.ComponentModel.DataAnnotations;

namespace DreamSoccer.Core.Dtos.User
{
    public class BuyPlayerRequest
    {
        [Required]
        public int TrasnferId { get; set; }
    }
}