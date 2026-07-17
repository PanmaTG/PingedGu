using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface INotificationsService
    {
        Task AddNewNotificationAsync(int userId, string message, string notificationType);
    }
}
