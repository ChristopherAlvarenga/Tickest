using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tickest.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public UsuariosController(UserManager<Usuario> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: Usuarios
        public IActionResult Index(string? email)
        {
            email = User.Identity.Name;

            var usuario = _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Especialidade)
                .Where(p => p.Email == email)
                .FirstOrDefault();

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var usuarios = await _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Especialidade)
                .Where(p => p.Id != 1).ToListAsync();

            var result = new List<UsuarioListViewModel>();

            foreach (var usuario in usuarios)
            {
                result.Add(new UsuarioListViewModel
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Especialidade = usuario.Especialidade?.Nome,
                    Departamento = usuario.Departamento?.Nome,
                    Cargo = string.Join(", ", await userManager.GetRolesAsync(usuario))
                });
            }

            return View(result);
        }

        [HttpGet]
        [Authorize(Roles = "Gerenciador")]
        public async Task<IActionResult> Edit(int? id)
        {
            var usuarioEntidade = await _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Especialidade)

                .FirstOrDefaultAsync(usuario => usuario.Id == id);

            var viewModel = new UsuarioEditViewModel();

            if (usuarioEntidade != null)
            {

                viewModel.Id = usuarioEntidade.Id;
                viewModel.Nome = usuarioEntidade.Nome;
                viewModel.Cargo = string.Join(", ", await userManager.GetRolesAsync(usuarioEntidade));

            }

            var query = _context.Departamentos
                .OrderBy(p => p.Nome)
                .AsQueryable();

            var query1 = _context.Especialidades
                .OrderBy(p => p.Nome)
                .AsQueryable();

            viewModel.Departamentos = query.Select(p => new Departamento
            {
                Id = p.Id,
                Nome = p.Nome,
                GerenciadorId = p.GerenciadorId
            }).ToList();

            viewModel.Especialidades = query1.Select(p => new Especialidade
            {
                Id = p.Id,
                Nome = p.Nome,
                DepartamentoId = p.DepartamentoId
            }).ToList();

            ViewBag.Departamentos = _context.Departamentos.OrderBy(p => p.Nome).ToList();
            ViewBag.Especialidades = new SelectList(query1);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Gerenciador")]
        public async Task<IActionResult> Edit(UsuarioEditViewModel viewModel)
        {
            //if (!ModelState.IsValid)
            //    return View(viewModel);

            var identityUser = await userManager.GetUserAsync(User);
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.Email == identityUser.Email);

            var usuarioEditar = await _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Especialidade)
                .FirstOrDefaultAsync(usuario => usuario.Id == viewModel.Id);

            bool isResponsavel = await userManager.IsInRoleAsync(identityUser, "Responsavel");
            bool mesmoDepartamento = usuarioEditar.DepartamentoId == usuarioLogado.DepartamentoId;

            if (isResponsavel && !mesmoDepartamento)
            {
                ViewBag.Error = "Não pode alterar usuario de outro departamento";
                return RedirectToAction(nameof(Edit));
            }

            usuarioEditar.Nome = viewModel.Nome;
            //usuarioEditar.Cargo = string.Join(", ", userManager.GetRolesAsync(usuarioLogado).GetAwaiter().GetResult());
            usuarioEditar.DepartamentoId = viewModel.DepartamentoId;
            usuarioEditar.EspecialidadeId = viewModel.EspecialidadeId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        [Authorize(Roles = "Gerenciador")]
        public async Task<IActionResult> Delete(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(List));
        }


        //// GET: Usuarios/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Usuarios == null)
        //    {
        //        return NotFound();
        //    }

        //    var usuario = await _context.Usuarios
        //        .Include(u => u.Cargo)
        //        .Include(u => u.Departamento)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (usuario == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(usuario);
        //}

        //// GET: Usuarios/Create
        //public IActionResult Create()
        //{
        //    ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome");
        //    ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Nome");
        //    return View();
        //}

        //// POST: Usuarios/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Nome,Email,CargoId,DepartamentoId")] Usuario usuario)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(usuario);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome", usuario.CargoId);
        //    ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Nome", usuario.DepartamentoId);
        //    return View(usuario);
        //}

        //// GET: Usuarios/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Usuarios == null)
        //    {
        //        return NotFound();
        //    }

        //    var usuario = await _context.Usuarios.FindAsync(id);
        //    if (usuario == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome", usuario.CargoId);
        //    ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Nome", usuario.DepartamentoId);
        //    return View(usuario);
        //}

        //// POST: Usuarios/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,CargoId,DepartamentoId")] Usuario usuario)
        //{
        //    if (id != usuario.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(usuario);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UsuarioExists(usuario.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Nome", usuario.CargoId);
        //    ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Nome", usuario.DepartamentoId);
        //    return View(usuario);
        //}

        //// GET: Usuarios/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Usuarios == null)
        //    {
        //        return NotFound();
        //    }

        //    var usuario = await _context.Usuarios
        //        .Include(u => u.Cargo)
        //        .Include(u => u.Departamento)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (usuario == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(usuario);
        //}

        //// POST: Usuarios/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Usuarios == null)
        //    {
        //        return Problem("Entity set 'TickestContext.Usuarios'  is null.");
        //    }
        //    var usuario = await _context.Usuarios.FindAsync(id);
        //    if (usuario != null)
        //    {
        //        _context.Usuarios.Remove(usuario);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool UsuarioExists(int id)
        //{
        //    return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        //}

    }
}
