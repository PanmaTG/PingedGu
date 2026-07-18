using Microsoft.EntityFrameworkCore;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public class AdminService : IAdminService
    {
        private readonly WebAppDbContext _context;

        public AdminService(WebAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetReportedPostsAsync()
        {
            var post = await _context.Posts
                .Where(n => n.NumOfReports > 5 && !n.IsDeleted)
                .ToListAsync();

            return posts;
        }
    }
}
