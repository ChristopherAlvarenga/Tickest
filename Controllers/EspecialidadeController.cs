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
    public class EspecialidadeController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public EspecialidadeController(UserManager<Usuario> userManager, TickestContext context)
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
                    GerenciadorId = p.GerenciadorId,
                    Usuarios = p.Usuarios,
                    Especialidades = p.Especialidades
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: EspecialidadeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DepartamentoId")] Especialidade Especialidade)
        {
            _context.Add(Especialidade);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Gerenciador");
        }
    }
}
