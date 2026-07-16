using Microsoft.EntityFrameworkCore;
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

        public async Task UpdateRequestAsync(int requestId, string newStatus)
        {
            var requestDb = await _context.FriendRequests.FirstOrDefaultAsync(n => n.Id == requestId);

            if (requestDb != null)
            {
                requestDb.Status = newStatus;
                requestDb.DateUpdated = DateTime.Now;
                _context.Update(requestDb);

                await _context.SaveChangesAsync();
            }

            if(newStatus == FriendshipStatus.Accepted)
            {
                var friendship = new Friendship
                {
                    SenderId = requestDb.SenderId,
                    ReceiverId = requestDb.ReceiverId,
                    DateCreated = DateTime.Now
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
    }
}
