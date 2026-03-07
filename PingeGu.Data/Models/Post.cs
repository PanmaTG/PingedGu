using System.ComponentModel.DataAnnotations;

namespace PingedGu.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public int NumOfReports { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsDeleted { get; set; }

        //FK = Foreign Key
        public int UserId { get; set; }

        public User User {  get; set; }

        // Nav Properties
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
