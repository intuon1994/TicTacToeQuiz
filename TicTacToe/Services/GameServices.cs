using System.Threading.Tasks;
using TicTacToe.Data.Table;
using TicTacToe.Models;
using TicTacToe.Repository;

namespace TicTacToe.Services
{
    public class GameServices:IGameServices
    {
        private readonly IGameRepository _gameRepo;
        private Random random = new Random();
        public GameServices(IGameRepository gameRepo) 
        { 
            _gameRepo = gameRepo;
        }
        public async Task<bool> SaveScores(GameState game, int change) => 
            await _gameRepo.SaveScores(game, change);
        public async Task<List<ScoreSummary>> GetScoreSummariesAll() => 
            await _gameRepo.GetScoreSummaryAll();
        public async Task<ScoreSummary?> GetScoreSummaryByPlayer(string player) => 
            await _gameRepo.GetScoreSummary(player);
        public async Task<ScoreTrackerModel> GetScoreTracker(int id) => 
            await _gameRepo.GetScoreTracker(id);
        public void ResetBoard(GameState game) => game.ResetBoard();
        public bool PlaceMove(GameState game, int row, int col)
        {
            if (game.Board[row, col] == ' ' && !game.GameOver)
            {
                game.Board[row, col] = game.CurrentPlayer;
                CheckGameOver(game);

                if (!game.GameOver)
                    SwitchPlayer(game);

                return true;
            }
            return false;
        }
        private void SwitchPlayer(GameState game)
        {
            game.CurrentPlayer = (game.CurrentPlayer == 'X') ? 'O' : 'X';

            if (game.VsBot && game.CurrentPlayer == 'O' && !game.GameOver)
            {
                BotMove(game);
            }
        }
        private void BotMove(GameState game)
        {
            // 1. ต้องหาแถวที่จะชนะให้ได้ก่อน
            var win = FindWinningMove(game, 'O');
            if (win != null)
            {
                MakeBotMove(game, win);
                return;
            }

            // 2. ไม่งั้นต้องบล็อค X
            var block = FindWinningMove(game, 'X');
            if (block != null)
            {
                MakeBotMove(game, block);
                return;
            }

            // 3. เลือกช่องแบบสุ่ม
            int row, col;
            do
            {
                row = random.Next(0, 3);
                col = random.Next(0, 3);
            } while (game.Board[row, col] != ' ');

            MakeBotMove(game, (row, col));
        }
        private void MakeBotMove(GameState game, (int, int)? move)
        {
            var (r, c) = move.Value;
            game.Board[r, c] = 'O';
            CheckGameOver(game);

            if (!game.GameOver)
                game.CurrentPlayer = 'X';
        }
        private void CheckGameOver(GameState game)
        {
            if (CheckWin(game, game.CurrentPlayer))
            {
                game.GameOver = true;

                if (game.CurrentPlayer == 'X')
                    HandlePlayerWin(game);
                else
                    HandleBotWin(game);
            }
            else if (IsDraw(game))
            {
                game.GameOver = true;
                game.Result = "Draw!";
            }
        }
        private async void HandlePlayerWin(GameState game)
        {
            int change = 1; // ปกติชนะ +1
            game.WinCount += 1;

            // ตรวจสอบ bonus ต้องชนะ 3 ครั้งติด
            if (game.WinCount >= 3)
            {
                change += 1; // โบนัส +1
                game.WinCount = 0; // รีเซ็ต streak
                game.Result = "You win! Score +1 and Bonus +1 (3 wins)";
            }
            else
            {
                game.Result = "You win! (+1 point)";
            }

            // SaveScore To db
            await SaveScores(game, change);
        }
        private async void HandleBotWin(GameState game)
        {
            int change = -1; // แพ้ -1
            game.WinCount = 0; // แพ้ streak รีเซ็ต
            game.Result = "Bot wins! (-1 point)";

            await SaveScores(game, change);
        }
        private bool CheckWin(GameState game, char player)
        {
            for (int i = 0; i < 3; i++)
                if (game.Board[i, 0] == player &&
                    game.Board[i, 1] == player &&
                    game.Board[i, 2] == player)
                    return true;

            for (int j = 0; j < 3; j++)
                if (game.Board[0, j] == player &&
                    game.Board[1, j] == player &&
                    game.Board[2, j] == player)
                    return true;

            if (game.Board[0, 0] == player &&
                game.Board[1, 1] == player &&
                game.Board[2, 2] == player)
                return true;

            if (game.Board[0, 2] == player &&
                game.Board[1, 1] == player &&
                game.Board[2, 0] == player)
                return true;

            return false;
        }
        private bool IsDraw(GameState game)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (game.Board[i, j] == ' ') return false;

            return true;
        }
        private (int, int)? FindWinningMove(GameState game, char player)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (game.Board[i, j] == ' ')
                    {
                        game.Board[i, j] = player;
                        if (CheckWin(game, player))
                        {
                            game.Board[i, j] = ' ';
                            return (i, j);
                        }
                        game.Board[i, j] = ' ';
                    }
                }
            }
            return null;
        }

    }
}
