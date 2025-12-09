namespace TicTacToe.Data.Table
{

    public class ScoreSummary
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }
        public int CurrentWinStreak { get; set; }
        public DateTime LastUpdated { get; set; }
    }  
}
