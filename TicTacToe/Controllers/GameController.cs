using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicTacToe.Data.Extensions;
using TicTacToe.Data.Identity;
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
            var _score = await _gameService.GetScoreSummary(User.Identity.GetUserName());
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
        public async Task<IActionResult> Move(int row, int col)
        {
            //var checkuser = await _userManager.FindByNameAsync(User.Identity.GetUserName());
            //if (checkuser != null) 
            //{
                game.Userlogin = User.Identity.GetUserName();
                _gameService.PlaceMove(game, row, col);
                return RedirectToAction("Index");
           // }

           // return RedirectToAction("Logout", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset()
        {
            _gameService.ResetBoard(game);
            return RedirectToAction("Index");
        }

        public IActionResult Score()
        {
            var abc = User.Identity.GetUserId();
            var www = User.Identity.GetUserName();
            var oooo = User.Identity.GetUserRole();
            return View();
        }
    }
}
