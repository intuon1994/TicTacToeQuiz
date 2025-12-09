using System;

namespace TicTacToe.Models
{
    public class GameBotPlayer
    {
        public char[,] Board { get; set; } = new char[3, 3];
        public char CurrentPlayer { get; set; } = 'X';
        public bool GameOver { get; set; } = false;
        public string Result { get; set; } = "";
        public bool VsBot { get; set; } = true; // ถ้า true เล่นกับ Bot

        public int PlayerScore { get; set; } = 0;      // คะแนนรวม
        public int WinCount { get; set; } = 0;         // จำนวนครั้งที่ชนะสะสม (เพื่อเช็คครบ 3)

        private Random random = new Random();

        public GameBotPlayer()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Board[i, j] = ' ';
        }

        public void ResetBoard()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Board[i, j] = ' ';

            GameOver = false;
            CurrentPlayer = 'X';
            Result = "";
        }

        public bool PlaceMove(int row, int col)
        {
            if (Board[row, col] == ' ' && !GameOver)
            {
                Board[row, col] = CurrentPlayer;
                CheckGameOver();

                if (!GameOver)
                    SwitchPlayer();

                return true;
            }
            return false;
        }

        private void CheckGameOver()
        {
            if (CheckWin(CurrentPlayer))
            {
                GameOver = true;

                // ผู้เล่น = X | Bot = O
                if (CurrentPlayer == 'X')
                {
                    // 1. ผู้เล่นชนะ → +1 คะแนน
                    PlayerScore += 1;

                    // 2. นับจำนวนครั้งที่ชนะ
                    WinCount += 1;

                    // 3. ถ้าชนะครบ 3 → โบนัส +1 และรีเซ็ตเคาน์เตอร์
                    if (WinCount >= 3)
                    {
                        PlayerScore += 1;   // โบนัส
                        WinCount = 0;       // รีเซ็ต
                        Result = "You win! Bonus +1 (3 wins)";
                    }
                    else
                    {
                        Result = "You win! (+1 point)";
                    }

                    // 4. Save ลง DB
                    //SaveScore();
                }
                else
                {
                    // ผู้เล่นแพ้ → -1 คะแนน
                    PlayerScore -= 1;
                    Result = "Bot wins! (-1 point)";

                    // ⛔ แพ้ไม่ต้องนับ WinCount  
                    //  Save ลง DB
                   // SaveScore();
                }
            }
            else if (IsDraw())
            {
                GameOver = true;
                Result = "Draw!";
            }
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == 'X') ? 'O' : 'X';
            if (VsBot && CurrentPlayer == 'O' && !GameOver)
            {
                BotMove();
            }
        }

        private void BotMove()
        {
            // 1. Check if Bot can win
            var winMove = FindWinningMove('O');
            if (winMove != null)
            {
                Board[winMove.Value.Item1, winMove.Value.Item2] = 'O';
                CheckGameOver();
                if (!GameOver) CurrentPlayer = 'X';
                return;
            }

            // 2. Check if Bot needs to block Player X
            var blockMove = FindWinningMove('X');
            if (blockMove != null)
            {
                Board[blockMove.Value.Item1, blockMove.Value.Item2] = 'O';
                CheckGameOver();
                if (!GameOver) CurrentPlayer = 'X';
                return;
            }

            // 3. Else pick random
            int row, col;
            do
            {
                row = random.Next(0, 3);
                col = random.Next(0, 3);
            } while (Board[row, col] != ' ');

            Board[row, col] = 'O';
            CheckGameOver();
            if (!GameOver) CurrentPlayer = 'X';
        }

        private bool CheckWin(char player)
        {
            // Row
            //[0,0] ,[0,1] , [0,2]
            for (int i = 0; i < 3; i++)
                if (Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                    return true;

            // Column
            for (int j = 0; j < 3; j++)
                if (Board[0, j] == player && Board[1, j] == player && Board[2, j] == player)
                    return true;

            // Diagonal
            if (Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
                return true;
            
            if (Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
                return true;

            return false;
        }

        private bool IsDraw()
        {
            //เสมอ เช็คเงื่อนไข ไม่มีช่องว่าง = true
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (Board[i, j] == ' ') return false;
            return true;
        }

        private (int, int)? FindWinningMove(char player)
        {
            // ลองทุกช่องว่าง
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Board[i, j] == ' ')
                    {
                        Board[i, j] = player;
                        if (CheckWin(player))
                        {
                            Board[i, j] = ' '; // กลับค่าเดิม
                            return (i, j);
                        }
                        Board[i, j] = ' ';
                    }
                }
            }
            return null;
        }
    }
}
