namespace PingedGu.ViewModels.Friends
{
    public class UserWithFriendsCountViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? PfpUrl { get; set; }
        public int FriendsCount { get; set; }
        public string FriendsCountDisplay => 
            FriendsCount == 0 ? "No followers" 
            : FriendsCount == 1 ? "1 follower" 
            : $"{FriendsCount} followers";

    }
}
