using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class DepartamentosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly TickestContext _context;

        public DepartamentosController(UserManager<IdentityUser> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        [Authorize(Policy = "RequireRole")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "RequireRole")]
        public IActionResult Create()
        {
            var query = _context.Usuarios.AsQueryable();

            var viewModel = new UsuarioViewModel()
            {
                Usuarios = query.Select(p => new Usuario
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Email = p.Email,
                    DepartamentoId = p.DepartamentoId
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Policy = "RequireRole")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nome,ResponsavelId")] Departamento departamento)
        {
                _context.Add(departamento);
                await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
        }
    }
}
