using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class DepartamentosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public DepartamentosController(UserManager<Usuario> userManager, TickestContext context)
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var usersGerenciador = await userManager.GetUsersInRoleAsync("Gerenciador");

            var viewModel = new DepartamentoEditViewModel()
            {
                Nome = string.Empty,
                GerenciadorSelecionado = 0,
                GerenciadorDisponiveis = usersGerenciador.Where(p => p.DepartamentoId == null).Select(p => new GerenciadorViewModel { Id = p.Id, Email = p.Email }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Create(DepartamentoEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var departamento = new Departamento();
            departamento.Nome = viewModel.Nome;
            departamento.GerenciadorId = viewModel.GerenciadorSelecionado.Value;

            _context.Add(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var gerenciadores = await userManager.GetUsersInRoleAsync("Gerenciador");

            var departamento = await _context.Set<Departamento>()
                 .Select(departamento => new DepartamentoEditViewModel
                 {
                     Id = departamento.Id,
                     Nome = departamento.Nome,
                     GerenciadorSelecionado = departamento.GerenciadorId,
                     GerenciadorDisponiveis = gerenciadores.Select(responsavel => new GerenciadorViewModel
                     {
                         Id = responsavel.Id,
                         Nome = responsavel.Email
                     }).ToList()
                 })
                 .FirstOrDefaultAsync(p => p.Id == id);

            return View(departamento);
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Edit(DepartamentoEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var departamento = await _context.Set<Departamento>().FirstAsync(p => p.Id == viewModel.Id);
            departamento.Nome = viewModel.Nome;
            departamento.GerenciadorId = viewModel.GerenciadorSelecionado.Value;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var departamentos = await _context.Set<Departamento>()
               .Select(departamento => new DepartamentoListViewModel
               {
                   Id = departamento.Id,
                   Nome = departamento.Nome,
                   Gerenciador = _context.Set<Usuario>().First(p => p.Id == departamento.GerenciadorId).Nome
               }).ToListAsync();

            return View(departamentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamentos()
        {
            var departamentos = await _context.Set<Departamento>()
                .Select(departamento => new DepartamentoListViewModel
                {
                    Id = departamento.Id,
                    Nome = departamento.Nome,
                    Gerenciador = _context.Set<Usuario>().First(p => p.Id == departamento.GerenciadorId).Nome
                }).ToListAsync();

            return Json(departamentos);
        }
    }
}
