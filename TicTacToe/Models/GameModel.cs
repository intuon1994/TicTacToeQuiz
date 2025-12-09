namespace TicTacToe.Models
{
    public class GameModel
    {
        public char[,] Board { get; set; } = new char[3, 3];
        public char CurrentPlayer { get; set; } = 'X';
        public bool GameOver { get; set; } = false;
        public string Result { get; set; } = "";
        public bool VsBot { get; set; } = true; // ถ้า true เล่นกับ Bot

        public GameModel()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Board[i, j] = ' ';
        }

        public bool PlaceMove(int row, int col)
        {
            if (Board[row, col] == ' ' && !GameOver)
            {
                Board[row, col] = CurrentPlayer;
                if (CheckWin(CurrentPlayer))
                {
                    GameOver = true;
                    Result = $"Player {CurrentPlayer} wins!";
                }
                else if (IsDraw())
                {
                    GameOver = true;
                    Result = "Draw!";
                }
                else
                {
                    CurrentPlayer = (CurrentPlayer == 'X') ? 'O' : 'X';
                }
                return true;
            }
            return false;
        }
        private bool CheckWin(char player)
        {
            // Row
            for (int i = 0; i < 3; i++)
                if (Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                    return true;

            // Column
            for (int j = 0; j < 3; j++)
                if (Board[0, j] == player && Board[1, j] == player && Board[2, j] == player)
                    return true;

            // Diagonals
            if (Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
                return true;
            if (Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
                return true;

            return false;
        }

        private bool IsDraw()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (Board[i, j] == ' ') return false;
            return true;
        }
    }
}

