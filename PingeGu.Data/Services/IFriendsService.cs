using PingedGu.Data.Dtos;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface IFriendsService
    {
        Task SendRequestAsync(int senderId, int receiverId);

        Task UpdateRequestAsync(int requestId, string status);
        Task RemoveFriendAsync(int friendshipId);
        Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId);
    }
}
