using PingedGu.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using PingedGu.Data.Hubs;
using PingedGu.Data.Helpers.Constants;

namespace PingedGu.Data.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly WebAppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationsService(WebAppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task AddNewNotificationAsync(int userId, string notificationType, string userFullname, int? postId)
        {
            var newNotification = new Notification()
            {
                UserId = userId,
                Message = GetPostMessage(notificationType, userFullname),
                Type = notificationType,
                IsRead = false,
                PostId = postId.HasValue ? postId.Value : null,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();

            var notificationNumber = await GetUnreadNotificationsCountAsync(userId);

            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notificationNumber);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            var count = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return count;
        }

        public async Task<List<Notification>> GetNotifications(int userId)
        {
            var allNotifications = await _context.Notifications.Where(n => n.UserId == userId)
                .OrderBy(n => n.IsRead)
                .ThenByDescending(n => n.DateCreated)
                .ToListAsync();

            return allNotifications;
        }

        public async Task SetNotificationAsReadAsync(int notificationId)
        {
            var notificationDb = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            if(notificationDb != null)
            {
                notificationDb.DateUpdated = DateTime.UtcNow;
                notificationDb.IsRead = true;

                _context.Notifications.Update(notificationDb);
                await _context.SaveChangesAsync();
            }
        }

        private string GetPostMessage(string notificationType, string userFullname)
        {
            var message = "";

            switch (notificationType)
            {
                case NotificationType.Like:
                    message = $"{userFullname} liked your post";
                    break;

                case NotificationType.Favorite:
                    message = $"{userFullname} saved your post";
                    break;

                case NotificationType.Comment:
                    message = $"{userFullname} posted a reply in your post";
                    break;

                case NotificationType.FriendRequest:
                    message = $"{userFullname} sent you a follow request";
                    break;

                case NotificationType.FriendRequestApproved:
                    message = $"{userFullname} accepted your follow request";
                    break;

                default:
                    message = "";
                    break;
            }

            return message;
        }
    }
}
