using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Services;
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
            return View(suggestedFriends);
        }
    }
}
