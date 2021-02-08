using DreamSoccer.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace DreamSoccer.Core.Dtos.User
{
    public class AddTransferListRequest 
    {
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public long Price { get; set; }

    }
}