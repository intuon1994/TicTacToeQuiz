using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Data;
using TicTacToe.Data.Identity;
using TicTacToe.Data.Table;
using TicTacToe.Models;

namespace TicTacToe.Repository
{
    public interface IGameRepository {
        Task<bool> SaveScores(GameState game, int change);
        Task<ScoreSummary?> GetScoreSummary(string player);
        Task<List<ScoreSummary>> GetScoreSummaryAll();
        Task SaveSummary(ScoreSummary summary);
        Task UpdateSummary(ScoreSummary summary);
        Task AddHistory(ScoreHistory history);
        Task<ScoreTrackerModel> GetScoreTracker(int id);
    }

    public class GameRepository:IGameRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public GameRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager) 
        { 
            _context = context;
            _userManager = userManager;
        }
        public async Task<ScoreSummary?> GetScoreSummary(string username)
        {
            return await _context.ScoreSummaries
                .FirstOrDefaultAsync(x => x.PlayerName == username);
        }
        public async Task<List<ScoreSummary>> GetScoreSummaryAll()
            => await _context.ScoreSummaries.ToListAsync();
        public async Task SaveSummary(ScoreSummary summary)
        {
            _context.ScoreSummaries.Add(summary);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateSummary(ScoreSummary summary)
        {
            _context.ScoreSummaries.Update(summary);
            await _context.SaveChangesAsync();
        }
        public async Task AddHistory(ScoreHistory history)
        {
            _context.ScoreHistories.Add(history);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> SaveScores(GameState game, int change)
        {
            var ischeckUser = await _userManager.FindByNameAsync(game.Userlogin);

            if (ischeckUser != null)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var summary = await GetScoreSummary(game.Userlogin);
                    if (summary == null)
                    {
                        summary = new ScoreSummary
                        {
                            PlayerName = game.Userlogin,
                            TotalScore = change,
                            CurrentWinStreak = game.WinCount,
                            LastUpdated = DateTime.Now
                        };
                        await SaveSummary(summary);
                    }
                    else
                    {
                        summary.TotalScore += change; // ใช้ change แทน PlayerScore
                        summary.CurrentWinStreak = game.WinCount;
                        summary.LastUpdated = DateTime.Now;
                        await UpdateSummary(summary);
                    }

                    // บันทึก History (ยกเว้น Draw)
                    if (!game.Result.Contains("Draw"))
                    {
                        var history = new ScoreHistory
                        {
                            PlayerName = game.Userlogin,
                            Result = game.Result,
                            ScoreChange = change,
                            TotalScoreAfter = summary.TotalScore,
                            Timestamp = DateTime.Now
                        };
                        await AddHistory(history);
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return false;
        }

        public async Task<ScoreTrackerModel> GetScoreTracker(int id) 
        {
            var _score = new ScoreTrackerModel();
            var scoreHistory = new List<ScoreHistory>();
            var scoreSummary = new ScoreSummary();

            scoreSummary = await _context.ScoreSummaries.FirstOrDefaultAsync(x=>x.Id == id);
            if (scoreSummary != null) {
                scoreHistory = await _context.ScoreHistories
                    .Where(x => x.PlayerName == scoreSummary.PlayerName)
                    .OrderByDescending(x => x.Timestamp).ToListAsync();
            }

            _score.ScoreSummary = scoreSummary;
            _score.ScoreDetail = scoreHistory;

            return _score;
        }
    }
}
