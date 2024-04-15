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
                ResponsavelSelecionado = 0,
                ResponsaveisDisponiveis = usersGerenciador.Where(p => p.DepartamentoId == null).Select(p => new ResponsavelViewModel { Id = p.Id, Email = p.Email }).ToList()
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
            departamento.ResponsavelId = viewModel.ResponsavelSelecionado.Value;

            _context.Add(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var responsaveisIdentity = await userManager.GetUsersInRoleAsync("Responsavel");
            var responsaveisIdentityEmails = responsaveisIdentity.Select(p => p.Email);

            var responsaveis = await _context.Set<Usuario>().Where(p => responsaveisIdentityEmails.Contains(p.Email)).ToListAsync();

            var departamento = await _context.Set<Departamento>()
                 .Select(departamento => new DepartamentoEditViewModel
                 {
                     Id = departamento.Id,
                     Nome = departamento.Nome,
                     ResponsavelSelecionado = departamento.ResponsavelId,
                     ResponsaveisDisponiveis = responsaveis.Select(responsavel => new ResponsavelViewModel
                     {
                         Id = responsavel.Id,
                         Nome = responsavel.Nome
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
            departamento.ResponsavelId = viewModel.ResponsavelSelecionado.Value;

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
                    Responsavel = _context.Set<Usuario> ().First(p=> p.Id == departamento.ResponsavelId).Nome
                }).ToListAsync();

            return View(departamentos);
        }
    }
}
