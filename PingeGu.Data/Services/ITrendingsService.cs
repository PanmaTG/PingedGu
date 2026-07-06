using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Services
{
    public interface ITrendingsService
    {
        Task ProcessTrendingsForNewPostAsync(string content);
        Task ProcessTrendingsForRemovedPostAsync(string content);
    }
}
