using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    //AUTHORIZE = USUÁRIO AUTENTICADO (APENAS USUÁRIOS AUTENTICADOS)
    [Authorize]
    public class DepartamentosController : Controller
    {
        private readonly TickestContext _context;

        public DepartamentosController(TickestContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "AdminGerenciadorPolicy")]
        public IActionResult Index()
        {
            return View();
        }

        //Dúvida
        [Authorize(Policy = "AdminGerenciadorPolicy")]
        public IActionResult Create()
        {
            var query = _context.Usuarios.AsQueryable();

            var viewModel = new UsuarioDepartamentoViewModel()
            {
                Usuarios = query.Select(p => new Usuario
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Email = p.Email,
                    CargoId = p.CargoId,
                    DepartamentoId = p.DepartamentoId
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult InserirDepartamento()
        {
            

            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminGerenciadorPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nome,ResponsavelId")] Departamento departamento)
        {
            _context.Add(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
        }
    }
}
