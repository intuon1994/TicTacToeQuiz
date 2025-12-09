using TicTacToe.Data.Table;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public interface IGameServices
    {
        Task<bool> SaveScores(GameState game);
        bool PlaceMove(GameState game, int row, int col);
        void ResetBoard(GameState game);
        Task<ScoreSummary?> GetScoreSummary(string player);
    }
}
