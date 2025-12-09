using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Data.Identity;
using TicTacToe.Data.Table;

namespace TicTacToe.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<ScoreHistory> ScoreHistories { get; set; }
        public DbSet<ScoreSummary> ScoreSummaries { get; set; }


    }
}
