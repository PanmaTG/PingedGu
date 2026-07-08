using Microsoft.EntityFrameworkCore;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public class UsersService : IUsersService
    {
        private readonly WebAppDbContext _webAppDbContext;

        public UsersService(WebAppDbContext webAppDbContext)
        {
            _webAppDbContext = webAppDbContext;
        }

        public async Task<User> GetUser(int loggedInUserId)
        {
            return await _webAppDbContext.Users.FirstOrDefaultAsync(n => n.Id == loggedInUserId) ?? new User();
        }
    }
}
