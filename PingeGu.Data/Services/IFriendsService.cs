using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface IFriendsService
    {
        Task<bool> SendRequest(int senderId, int receiverId);
    }
}
