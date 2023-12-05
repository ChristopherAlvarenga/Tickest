using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tickest.Models;
using Tickest.Services.Authentication;

namespace Tickest.Controllers
{
    [Authorize]
    public class ColaboradoresController : Controller
    {
        private readonly ILogger<ColaboradoresController> _logger;
        private readonly IAccountService _authenticationService;

        public ColaboradoresController(
            ILogger<ColaboradoresController> logger,
            IAccountService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}