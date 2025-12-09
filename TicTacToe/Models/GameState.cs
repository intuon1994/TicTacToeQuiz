namespace TicTacToe.Models
{
    public class GameState
    {
        public char[,] Board { get; set; } = new char[3, 3];
        public char CurrentPlayer { get; set; } = 'X';
        public bool GameOver { get; set; } = false;
        public string Result { get; set; } = "";
        public bool VsBot { get; set; } = true;
        public int PlayerScore { get; set; } = 0;
        public int WinCount { get; set; } = 0;
        public string Userlogin { get; set; }
    
        public GameState()
        {
            ResetBoard();
        }
        public void ResetBoard()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Board[i, j] = ' ';

            CurrentPlayer = 'X';
            GameOver = false;
            Result = "";
        }
    }
}
