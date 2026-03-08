using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? PfpUrl { get; set; }
        public bool IsDeleted { get; set; }

        // Nav Properties
        public ICollection<Story> Stories { get; set; } = new List<Story>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
