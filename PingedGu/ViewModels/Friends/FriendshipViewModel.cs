using PingedGu.Data.Models;

namespace PingedGu.ViewModels.Friends
{
    public class FriendshipViewModel
    {
        public List<FriendRequest> FriendRequestsSent = new List<FriendRequest>();
        public List<FriendRequest> FriendRequestsReceived = new List<FriendRequest>();
    }
}
