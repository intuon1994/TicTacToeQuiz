using TicTacToe.Data.Table;

namespace TicTacToe.Models
{
    public class ScoreTrackerModel
    {
        public ScoreSummary ScoreSummary { get; set; }
        public List<ScoreHistory> ScoreDetail { get; set; }
    }
}
