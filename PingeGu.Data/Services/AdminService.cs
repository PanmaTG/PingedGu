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
            var posts = await _context.Posts
                .Include(n => n.User)
                .Where(n => n.NumOfReports > 5 && !n.IsDeleted)
                .ToListAsync();

            return posts;
        }

        public async Task ApproveReportAsync(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);

            if(postDb != null)
            {
                postDb.IsDeleted = true;
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectReportAsync(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);

            if (postDb != null)
            {
                postDb.NumOfReports = 0;
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }

            var postReports = await _context.Reports.Where(n => n.PostId == postId)
                .ToListAsync();
            if (postReports.Any())
            {
                _context.Reports.RemoveRange(postReports);
                await _context.SaveChangesAsync();
            }
        }
    }
}
