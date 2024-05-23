using Microsoft.AspNetCore.Mvc;
using MyBiz_Business.Services.Abstracts;
using MyBiz_Core.Models;
using System.Diagnostics;

namespace MyBiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITeamService _teamService;

        public HomeController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public IActionResult Index()
        {
            List<Team> teams = _teamService.GetAllTeams();
            return View(teams);
        }
       
    }
}
