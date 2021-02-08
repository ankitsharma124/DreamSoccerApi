using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamSoccer.Core.Entities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool DelFlag { get; set; } = false;
    }
}