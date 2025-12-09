using Microsoft.EntityFrameworkCore;
using TicTacToe.Data;
using TicTacToe.Data.Table;
using TicTacToe.Models;

namespace TicTacToe.Repository
{
    public interface IGameRepository {
        Task<bool> SaveScores(GameState game);
        Task<ScoreSummary?> GetScoreSummary(string player);
        Task<List<ScoreSummary>> GetScoreSummaryAll();
        Task SaveSummary(ScoreSummary summary);
        Task UpdateSummary(ScoreSummary summary);
        Task AddHistory(ScoreHistory history);
    }

    public class GameRepository:IGameRepository
    {
        private readonly ApplicationDbContext _context;
        public GameRepository(ApplicationDbContext context) 
        { 
            _context = context;
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

        public async Task<bool> SaveScores(GameState game)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int change = 0;

                if (game.Result.Contains("Bot wins"))
                    change = -1;
                else if (game.Result.Contains("Bonus"))
                    change = +1;
                else if (game.Result.Contains("win"))
                    change = +1;

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
                    summary.TotalScore += change;        // ใช้ change
                    summary.CurrentWinStreak = game.WinCount;
                    summary.LastUpdated = DateTime.Now;

                    await UpdateSummary(summary);
                }

                
                    var history = new ScoreHistory
                    {
                        PlayerName = game.Userlogin,
                        Result = game.Result,
                        ScoreChange = change,
                        TotalScoreAfter = summary.TotalScore,
                        Timestamp = DateTime.Now
                    };

                    await AddHistory(history);
                

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
