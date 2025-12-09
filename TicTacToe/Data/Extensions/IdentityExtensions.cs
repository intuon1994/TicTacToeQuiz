using System.Security.Claims;
using System.Security.Principal;

namespace TicTacToe.Data.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim != null ? claim.Value : ""; //"[Unknown user id]";

            return userId;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Name);
            string userName = claim != null ? claim.Value : "";

            return userName;
        }
        public static string GetUserRole(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Role);
            string userRole = claim != null ? claim.Value : "";

            return userRole;
        }
    }
}
