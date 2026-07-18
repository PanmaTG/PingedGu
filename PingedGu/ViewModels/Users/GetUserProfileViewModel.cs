using PingedGu.Data.Dtos;
using PingedGu.Data.Models;

namespace PingedGu.ViewModels.Users
{
    public class GetUserProfileViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
        public List<UserWithFriendsCountDto> Friends { get; set; } = new();
    }
}
