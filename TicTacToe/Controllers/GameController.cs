using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicTacToe.Data.Extensions;
using TicTacToe.Data.Identity;
using TicTacToe.Data.Table;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IGameServices _gameService;
        private static GameState game = new GameState();
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(
            IGameServices gameService,
            UserManager<ApplicationUser> userManager) { 
            _gameService = gameService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var _score = await _gameService.GetScoreSummaryByPlayer(User.Identity.GetUserName());
            if (_score != null)
            {
                game.PlayerScore = _score.TotalScore;
            }
            else {
                game.PlayerScore = 0;
            }

           return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Move(int row, int col)
        {
            game.Userlogin = User.Identity.GetUserName();
            _gameService.PlaceMove(game, row, col);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset()
        {
            _gameService.ResetBoard(game);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdministratorOnly")]
        [HttpGet]
        public IActionResult ScoreTracker() => View();

        [Authorize(Policy = "AdministratorOnly")]
        [HttpGet]
        public async Task<IActionResult> ScoreTrackerDetail(int id) {
           
            var scoreTracker = await _gameService.GetScoreTracker(id);
            return View(scoreTracker);
        }

        public class DataTableScoreSummary
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<ScoreSummary> data { get; set; }
        }

        [Authorize(Policy = "AdministratorOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoadScoreSummaryList()
        {
            var draw = HttpContext.Request.Form["draw"].ToString();
            var start = HttpContext.Request.Form["start"].ToString();
            var length = HttpContext.Request.Form["length"].ToString();
            var sortColumn = HttpContext.Request.Form["columns[" + HttpContext.Request.Form["order[0][column]"].ToString() + "][name]"].ToString();
            var sortColumnDir = HttpContext.Request.Form["order[0][dir]"].ToString();
            string search = HttpContext.Request.Form["search[value]"];

            DataTableScoreSummary dataTable = await FilterData(Convert.ToInt32(draw), Convert.ToInt32(start), Convert.ToInt32(length), search, sortColumn, sortColumnDir);
            return Json(dataTable);
        }
        private async Task<DataTableScoreSummary> FilterData(int draw, int start, int length, string search, string sortColumn, string sortColumnDir)
        {
            List<ScoreSummary> data = new List<ScoreSummary>();

            var summary = await _gameService.GetScoreSummariesAll();

            if (!string.IsNullOrEmpty(search))
            {
                summary = summary.Where(e =>
                e.PlayerName.ToLower().Contains(search.ToLower())).ToList();
            }

            switch (sortColumn)
            {
                case "PlayerName":
                    summary = sortColumnDir == "asc" ? summary.OrderBy(e => e.PlayerName).ToList() : summary.OrderByDescending(e => e.PlayerName).ToList();
                    break;
                case "TotalScore":
                    summary = sortColumnDir == "asc" ? summary.OrderBy(e => e.TotalScore).ToList() : summary.OrderByDescending(e => e.TotalScore).ToList();
                    break;
                case "CurrentWinStreak":
                    summary = sortColumnDir == "asc" ? summary.OrderBy(e => e.CurrentWinStreak).ToList() : summary.OrderByDescending(e => e.CurrentWinStreak).ToList();
                    break;
                case "LastUpdated":
                    summary = sortColumnDir == "asc" ? summary.OrderBy(e => e.LastUpdated).ToList() : summary.OrderByDescending(e => e.LastUpdated).ToList();
                    break;
                default:
                    summary = summary.OrderByDescending(e => e.LastUpdated).ToList();
                    break;
            }

            foreach (var item in summary.Skip(start).Take(length).ToList())
            {
                data.Add(new ScoreSummary()
                {
                    Id = item.Id,
                    PlayerName = item.PlayerName,
                    TotalScore = item.TotalScore,
                    CurrentWinStreak = item.CurrentWinStreak,
                    LastUpdated = item.LastUpdated,
                });
            }

            int recordsTotal = summary.Count();
            DataTableScoreSummary dataTable = new DataTableScoreSummary
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data
            };

            return dataTable;
        }
    }
}
