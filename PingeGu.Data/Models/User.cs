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

        //Nav Properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
