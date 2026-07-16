using Microsoft.AspNetCore.Mvc;
using PingedGu.Controllers.Base;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Friends;

namespace PingedGu.Controllers
{
    public class FriendsController : BaseController
    {
        public readonly IFriendsService _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                RedirectToLogin();
            }

            var friendsData = new FriendshipViewModel()
            {
                FriendRequestSent = await _friendsService.GetSentFriendRequestsAsync(userId.Value)
            };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                RedirectToLogin();
            }

            await _friendsService.SendRequestAsync(userId.Value, receiverId);
            return RedirectToAction("Index", "Home");
        }
    }
}
