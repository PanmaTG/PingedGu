using Microsoft.AspNetCore.Http;
using PingedGu.Data.Dtos;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface IPostsService
    {
        Task<List<Post>> GetAllPostsAsync(int loggedInUserId);
        Task<Post> GetPostByIdAsync(int postId);
        Task<List<Post>> GetAllFavoritedPostsAsync(int loggedInUserId);
        Task<Post> CreatePostAsync(Post post);
        Task<Post> RemovePostAsync(int postId);

        Task<bool> AddPostCommentAsync(Comment comment);
        Task RemovePostCommentAsync(int commentId);

        Task<GetNotificationDto> TogglePostLikeAsync(int postId, int userId);
        Task<GetNotificationDto> TogglePostFavoriteAsync(int postId, int userId);
        Task TogglePostVisibilityAsync(int postId, int userId);
        Task ReportPostAsync(int postId, int userId);
    }
}
