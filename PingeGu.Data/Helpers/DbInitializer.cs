using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Helpers
{
    public static class DbInitializer
    {
        //To make this an async change void → Task
        //add async keyword before "Task" — public static async Task SeedAsync();
        //& change Seed → SeedAsync
        public static async Task SeedAsync(WebAppDbContext webAppDbContext)
        {
            if (!webAppDbContext.Users.Any() && !webAppDbContext.Posts.Any())
            {
                var newUser = new User()
                {
                    FullName = "TripleEarthhh",
                    PfpUrl = "https://preview.redd.it/whos-your-favorite-sora-v0-2g7mjaz3t64f1.jpg?width=554&format=pjpg&auto=webp&s=47aa5e4e7c0dd58d91743a1841db9308bd1c505a"
                };

                //To make this in async add "await keyword at the beginning here
                //change Add() → AddAsync();
                //& change SaveChanges () → SaveChangesAsync();
                await webAppDbContext.Users.AddAsync(newUser);
                await webAppDbContext.SaveChangesAsync();

                var newPostWithoutImage = new Post()
                {
                    Content = "First post by PanmaTG lets go. 676767676",
                    ImageUrl = "",
                    NumOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                var newPostWithImage = new Post()
                {
                    Content = "First post by PanmaTG lets go. 676767676. Image post baby",

                    ImageUrl = "https://static.toiimg.com/thumb/msid-120058075,width-1280,height-720,imgsize-65786,resizemode-6,overlay-toi_sw,pt-32,y_pad-40/photo.jpg",
                    NumOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                //To make this in async add "await keyword at the beginning here
                //change AddRange() → AddRangeAsync();
                //& change SaveChanges () → SaveChangesAsync();
                await webAppDbContext.Posts.AddRangeAsync(newPostWithoutImage, newPostWithImage);
                await webAppDbContext.SaveChangesAsync();
            }
        }
    }
}
