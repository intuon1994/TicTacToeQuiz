using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Data.Identity
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public DateTime CreateTimestamp { get; set; }
        public DateTime? LastAccess { get; set; }
        public bool IsActive { get; set; }
    }
}
