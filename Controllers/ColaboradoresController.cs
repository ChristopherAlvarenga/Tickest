using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tickest.Models;

namespace Tickest.Controllers
{

    [Authorize]
    public class ColaboradoresController : Controller
    {
        private readonly ILogger<ColaboradoresController> _logger;

        public ColaboradoresController(ILogger<ColaboradoresController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
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