using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PingedGu.Data.Models
{
    public class Story
    {
        public int Id { get; set; }

        public string? ImageUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }

        //FK = Foreign Key
        public int UserId { get; set; }

        // Nav Properties
        public User User { get; set; }
    }
}
