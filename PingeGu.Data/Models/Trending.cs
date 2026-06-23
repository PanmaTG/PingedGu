using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Models
{
    public class Trending
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
