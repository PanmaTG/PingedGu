using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly WebAppDbContext _context;

        public NotificationsService(WebAppDbContext context)
        {
            _context = context;
        }

        public async Task AddNewNotificationAsync(int userId, string message, string notificationType)
        {
            var newNotification = new Notification()
            {
                UserId = userId,
                Message = message,
                Type = notificationType,
                IsRead = false,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }
    }
}
