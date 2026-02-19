using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        //FK = Foreign Key
        public int PostId { get; set; }
        public int UserId { get; set; }

        // Nav Properties
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
