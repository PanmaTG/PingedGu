using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PingedGu.Data.Helpers
{
    public static class TrendingHelper
    {
        public static List<string> GetTrendings(string postText)
        {
            var trendingPattern = new Regex(@"#\w+");
            var matches = trendingPattern.Matches(postText)
                .Select(match => match.Value.TrimEnd('.', ',', '!', '?').ToLower())
                .Distinct()
                .ToList();

            return matches;
        }
    }
}
