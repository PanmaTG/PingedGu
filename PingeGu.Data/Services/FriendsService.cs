using PingedGu.Data.Helpers.Constants;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly WebAppDbContext _context;

        public FriendsService(WebAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SendRequest(int senderId, int receiverId)
        {
            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
