using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AdminPanel.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdminPanel.Models;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IProjectService projectService, UserManager<ApplicationUser> userManager)
        {
            _projectService = projectService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var dashboardData = await _projectService.GetDashboardDataAsync(userId);
            return View(dashboardData);
        }
    }
}
