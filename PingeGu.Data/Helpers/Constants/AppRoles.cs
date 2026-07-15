using System;
using System.Collections.Generic;
using System.Text;

namespace PingedGu.Data.Helpers.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly IReadOnlyList<string> All = new[] { Admin, User };
    }
}
