using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Friends;
using System.Security.Claims;

namespace PingedGu.ViewComponents
{
    public class SuggestedForYouViewComponent : ViewComponent
    {
        private readonly IFriendsService _friendsService;

        public SuggestedForYouViewComponent(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.Parse(loggedInUserId);

            var suggestedFriends = await _friendsService.GetSuggestedFriendsAsync(userId);
            var suggestedFriendsViewModel = suggestedFriends.Select(n => new UserWithFriendsCountViewModel()
            {
                UserId = n.User.Id,
                FullName = n.User.FullName,
                PfpUrl = n.User.PfpUrl,
                FriendsCount = n.FriendsCount
            }).ToList();

            return View(suggestedFriendsViewModel);
        }
    }
}
