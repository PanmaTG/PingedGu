using PingedGu.Data.Models;

namespace PingedGu.ViewModels.Friends
{
    public class FriendshipViewModel
    {
        public List<Friendship> Friends = new List<Friendship>();
        public List<FriendRequest> FriendRequestsSent = new List<FriendRequest>();
        public List<FriendRequest> FriendRequestsReceived = new List<FriendRequest>();
    }
}
