using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PingedGu.Data.Dtos;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public class PostsService:IPostsService
    {
        private readonly WebAppDbContext _context;
        private readonly INotificationsService _notificationsService;

        public PostsService(WebAppDbContext context, INotificationsService notificationsService)
        {
            _context = context;
            _notificationsService = notificationsService;
        }

        public async Task<List<Post>> GetAllPostsAsync(int loggedInUserId)
        {
            var allPosts = await _context.Posts
                .Where(n => (!n.IsPrivate || n.UserId == loggedInUserId) 
                    && n.Reports.Count < 5 
                    && !n.IsDeleted)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments)
                    .ThenInclude(n => n.User)
                .Include(n => n.Reports)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();

            return allPosts;
        }

        public async Task<Post> GetPostByIdAsync (int postId)
        {
            var postDb = await _context.Posts
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments)
                    .ThenInclude(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == postId);

            return postDb;
        }

        public async Task<List<Post>> GetAllFavoritedPostsAsync(int loggedInUserId)
        {
            var allFavoritedPosts = await _context.Favorites
                .Include(f => f.Post.Reports)
                .Include(f => f.Post.User)
                .Include(f => f.Post.Comments)
                    .ThenInclude(c => c.User)
                .Include(f => f.Post.Likes)
                .Include(f => f.Post.Favorites)
                .Where(n => n.UserId == loggedInUserId
                    && !n.Post.IsDeleted
                    && n.Post.Reports.Count < 5)
                .OrderByDescending(f => f.DateCreated)
                .Select(n => n.Post)
                .ToListAsync();

            return allFavoritedPosts;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {


            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> RemovePostAsync(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);

            if (postDb != null)
            {
                //_context.Posts.Remove(postDb);
                postDb.IsDeleted = true;
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }

            return postDb;
        }

        public async Task<bool> AddPostCommentAsync(Comment comment)
        {
            if (comment == null || string.IsNullOrWhiteSpace(comment.Content))
                return false;

            var postExists = await _context.Posts
                .AnyAsync(p => p.Id == comment.PostId && !p.IsDeleted);

            if (!postExists)
                return false;

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task RemovePostCommentAsync(int commentId)
        {
            var commentDb = _context.Comments.FirstOrDefault(n => n.Id == commentId);

            if (commentDb != null) 
            { 
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReportPostAsync(int postId, int userId)
        {
            var newReport = new Report()
            {
                PostId = postId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();

            var post = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);
            if(post != null)
            {
                post.NumOfReports += 1;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GetNotificationDto> TogglePostFavoriteAsync(int postId, int userId)
        {
            var response = new GetNotificationDto()
            {
                Success = true,
                SendNotification = false
            };

            var favorite = await _context.Favorites
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Already removed by a concurrent request — nothing to do
                }
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postId,
                    UserId = userId,
                    DateCreated = DateTime.UtcNow
                };

                await _context.Favorites.AddAsync(newFavorite);

                try
                {
                    await _context.SaveChangesAsync();
                    response.SendNotification = true;
                }
                catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
                {
                    _context.Entry(newFavorite).State = EntityState.Detached;
                    response.SendNotification = false;
                }
            }

            return response;
        }

        public async Task<GetNotificationDto> TogglePostLikeAsync(int postId, int userId)
        {
            var response = new GetNotificationDto()
            {
                Success = true,
                SendNotification = false
            };

            var like = await _context.Likes
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                _context.Likes.Remove(like);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) 
                {
                }
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postId,
                    UserId = userId
                };

                await _context.Likes.AddAsync(newLike);

                try
                {
                    await _context.SaveChangesAsync();
                    response.SendNotification = true;
                }
                catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
                {
                    _context.Entry(newLike).State = EntityState.Detached;
                    response.SendNotification = false;
                }
            }

            return response;
        }

        private static bool IsDuplicateKeyViolation(DbUpdateException ex) // ← NEW, add as a private method in the same class
        {
            return ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                && (sqlEx.Number == 2627 || sqlEx.Number == 2601);
        }

        public async Task TogglePostVisibilityAsync(int postId, int userId)
        {
            // Get post id and loggedin user id
            var post = await _context.Posts
                .FirstOrDefaultAsync(l => l.Id == postId && l.UserId == userId);

            if (post != null)
            {
                post.IsPrivate = !post.IsPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
