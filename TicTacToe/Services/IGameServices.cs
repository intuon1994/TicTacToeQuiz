using TicTacToe.Data.Table;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public interface IGameServices
    {
        Task<bool> SaveScores(GameState game, int change);
        bool PlaceMove(GameState game, int row, int col);
        void ResetBoard(GameState game);
        Task<ScoreSummary?> GetScoreSummaryByPlayer(string player);
        Task<List<ScoreSummary>> GetScoreSummariesAll();
        Task<ScoreTrackerModel> GetScoreTracker(int id);
    }
}
