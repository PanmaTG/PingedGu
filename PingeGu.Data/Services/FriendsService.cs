using Microsoft.EntityFrameworkCore;
using PingedGu.Data.Dtos;
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

        public async Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId)
        {
            //Get existing follower ids
            var existingFriendsIds = await _context.Friendships
                .Where(n => n.SenderId == userId || n.ReceiverId == userId)
                .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
                .ToListAsync();

            //Pending Requests
            var pendingRequestsIds = await _context.FriendRequests
                .Where(n => (n.SenderId == userId || n.ReceiverId == userId) && n.Status == FriendshipStatus.Pending)
                .Select(n => n.SenderId == userId ? n.ReceiverId : n.SenderId)
                .ToListAsync();

            //Get Suggested Followers
            var suggestedFriends = await _context.Users
                .Where(n => n.Id != userId &&
                !existingFriendsIds.Contains(n.Id) && 
                !pendingRequestsIds.Contains(n.Id))
                .Select(u => new UserWithFriendsCountDto() 
                {
                    User = u,
                    FriendsCount = _context.Friendships
                        .Count(f => f.SenderId == u.Id || f.ReceiverId == u.Id)
                })
                .Take(5)
                .ToListAsync();

            return suggestedFriends;
        }

        public async Task UpdateRequestAsync(int requestId, string newStatus)
        {
            var requestDb = await _context.FriendRequests.FirstOrDefaultAsync(n => n.Id == requestId);

            if (requestDb != null)
            {
                requestDb.Status = newStatus;
                requestDb.DateUpdated = DateTime.UtcNow;
                _context.Update(requestDb);

                await _context.SaveChangesAsync();
            }

            if(newStatus == FriendshipStatus.Accepted)
            {
                var friendship = new Friendship
                {
                    SenderId = requestDb.SenderId,
                    ReceiverId = requestDb.ReceiverId,
                    DateCreated = DateTime.UtcNow
                };
                await _context.Friendships.AddAsync(friendship);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendRequestAsync(int senderId, int receiverId)
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
        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(n => n.Id == friendshipId);

            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FriendRequest>> GetSentFriendRequestsAsync(int userId)
        {
            var friendRequestsSent = await _context.FriendRequests
                .Include(n => n.Sender)
                .Include(n => n.Receiver)
                .Where(f => f.SenderId == userId && f.Status == FriendshipStatus.Pending)
                .ToListAsync();

            return friendRequestsSent;
        }

        public async Task<List<FriendRequest>> GetReceivedFriendRequestAsync(int userId)
        {
            var friendRequestsSent = await _context.FriendRequests
                .Include(n => n.Sender)
                .Include(n => n.Receiver)
                .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
                .ToListAsync();

            return friendRequestsSent;
        }
    }
}
