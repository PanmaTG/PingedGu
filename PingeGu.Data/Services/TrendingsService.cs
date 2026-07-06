using Microsoft.EntityFrameworkCore;
using PingedGu.Data.Helpers;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public class TrendingsService : ITrendingsService
    {
        private readonly WebAppDbContext _context;
        public TrendingsService(WebAppDbContext context) 
        {
            _context = context;
        }

        public async Task ProcessTrendingsForNewPostAsync(string content)
        {
            //Searches for hashtags in a post and stores them in the database
            var postTrendings = TrendingHelper.GetTrendings(content);
            foreach (var trending in postTrendings)
            {
                var trendingDb = await _context.Trendings.FirstOrDefaultAsync(n => n.Name == trending);
                if (trendingDb != null)
                {
                    trendingDb.Count += 1;
                    trendingDb.DateUpdated = DateTime.UtcNow.AddHours(8);

                    _context.Trendings.Update(trendingDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newTrending = new Trending()
                    {
                        Name = trending,
                        Count = 1,
                        DateCreated = DateTime.UtcNow.AddHours(8),
                        DateUpdated = DateTime.UtcNow.AddHours(8)
                    };

                    await _context.Trendings.AddAsync(newTrending);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ProcessTrendingsForRemovedPostAsync(string content)
        {
            //Update trendings count if post is deleted
            var postTrendings = TrendingHelper.GetTrendings(content);
            foreach (var trending in postTrendings)
            {
                var trendingDb = await _context.Trendings.FirstOrDefaultAsync(n => n.Name == trending);
                if (trendingDb != null)
                {
                    trendingDb.Count -= 1;
                    trendingDb.DateUpdated = DateTime.UtcNow;
                    trendingDb.DateUpdated = DateTime.UtcNow;

                    _context.Trendings.Update(trendingDb);
                    await _context.SaveChangesAsync();
                }

            }
        }
    }
}
