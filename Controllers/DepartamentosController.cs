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

            foreach (var user in usersResponsavel)
            {
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

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nome,ResponsavelId")] Departamento departamento)
        {
            _context.Add(departamento);
            await _context.SaveChangesAsync();

            return View();
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
            if (string.IsNullOrEmpty(viewModel.Nome))
                ModelState.AddModelError(nameof(DepartamentoEditViewModel.Nome), "Nome não informado");

            //Continuar validação

            if (!ModelState.IsValid)
                return View(viewModel);

            var departamento = await _context.Set<Departamento>().FirstAsync(p => p.Id == viewModel.Id);
            departamento.Nome = viewModel.Nome;
            departamento.ResponsavelId = viewModel.ResponsavelSelecionado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var departamentos = await _context.Set<Departamento>()
                .Select(departamento => new DepartamentoListViewModel
                {
                    Id = departamento.Id,
                    Nome = departamento.Nome,
                    Responsavel = _context.Set<Usuario>().Select(responsavel => new ResponsavelViewModel
                    {
                        Id = responsavel.Id,
                        Nome = responsavel.Nome
                    }).FirstOrDefault(x => x.Id == departamento.ResponsavelId)
                }).ToListAsync();

            return View(departamentos);
        }
    }
}
