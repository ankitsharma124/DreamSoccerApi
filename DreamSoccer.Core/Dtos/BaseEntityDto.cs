using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamSoccer.Core.Dtos
{
    public class BaseEntityDto
    {
        public int Id { get; set; }
        public bool DelFlag { get; set; } = false;
    }
}