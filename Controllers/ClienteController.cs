using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tickest.Data;
using Tickest.Models.Entities;

namespace Tickest.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TickestContext _context;

        public ClienteController(UserManager<Usuario> userManager, TickestContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index() => View();
    }
}
