using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface IUsersService
    {
        Task<User> GetUser(int loggedInUserId);
    }
}
