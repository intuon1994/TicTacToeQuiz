using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Data.Table
{
    public class ScoreHistory
    {
        [Key]
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string Result { get; set; }
        public int ScoreChange { get; set; }
        public int TotalScoreAfter { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
