using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface IAdminService
    {
        Task<List<Post>> GetReportedPostsAsync();
        Task ApproveReportAsync(int postId);
        Task RejectReportAsync(int postId);
    }
}
