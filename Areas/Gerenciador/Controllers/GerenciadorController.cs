using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tickest.Controllers
{
    [Area("Gerenciador")]
    [Authorize(Policy = "RequireRole")]
    public class GerenciadorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
