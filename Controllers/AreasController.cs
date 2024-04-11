using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize(Roles = "Gerenciador")]
    public class AreasController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public AreasController(UserManager<Usuario> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        public IActionResult Create()
        {
            var query = _context.Departamentos
                .AsQueryable();

            var viewModel = new UsuarioViewModel()
            {
                Departamentos = query.Select(p => new Departamento
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    ResponsavelId = p.ResponsavelId,
                    Usuarios = p.Usuarios,
                    Areas = p.Areas
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: AreasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DepartamentoId")] Area area)
        {
            _context.Add(area);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Gerenciador");
        }
    }
}
