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

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Gerenciador")]
        public IActionResult Create()
        {
            var usersResponsavel = userManager
                .GetUsersInRoleAsync("Responsavel").Result;

            List<Usuario> users = new List<Usuario>();

            foreach (var user in usersResponsavel) {
                var contextUsers = _context.Usuarios
                    .Where(p => p.Email == user.Email)
                    .FirstOrDefault();

                users.Add(contextUsers);
            }

            var query = users.AsQueryable();

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

        [Authorize(Policy = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nome,ResponsavelId")] Departamento departamento)
        {
                _context.Add(departamento);
                await _context.SaveChangesAsync();

            return View();
        }
    }
}
