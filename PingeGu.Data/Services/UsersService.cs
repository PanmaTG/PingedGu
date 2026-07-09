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

        public async Task UpdateUserProfilePicture(int loggedInUserId, string pfpUrl)
        {
            var userDb = await _webAppDbContext.Users.FirstOrDefaultAsync(n => n.Id == loggedInUserId);

            if (userDb != null)
            {
                userDb.PfpUrl = pfpUrl;
                _webAppDbContext.Users.Update(userDb);
                await _webAppDbContext.SaveChangesAsync();
            }
        }
    }
}
