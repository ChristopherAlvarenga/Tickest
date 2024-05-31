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
        private readonly UserManager<IdentityUser> userManager;
        private readonly TickestContext _context;

        public UsuariosController(UserManager<IdentityUser> userManager, TickestContext context)
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
                .Include(p => p.Area)
                .Where(p => p.Email == email).FirstOrDefault();

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var usuarios = await _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Area)
                .Where(p => p.Id != 1)
                .Select(usuario => new UsuarioListViewModel
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Area = usuario.Area.Nome,
                    Departamento = usuario.Departamento.Nome,
                    Cargo = usuario.Cargo
                })
                .ToListAsync();

            return View(usuarios);
        }

        [HttpGet]
        [Authorize(Roles = "Gerenciador, Responsavel")]
        public async Task<IActionResult> Edit(int? id)
        {
            var usuario = await _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Area)
                .Select(usuario => new UsuarioEditViewModel
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Cargo = usuario.Cargo
                })
                .FirstOrDefaultAsync(usuario => usuario.Id == id);

            var query = _context.Departamentos
                .OrderBy(p => p.Nome)
                .AsQueryable();

            var query1 = _context.Areas
                .OrderBy(p => p.Nome)
                .AsQueryable();

            usuario.Departamentos = query.Select(p => new Departamento
            {
                Id = p.Id,
                Nome = p.Nome,
                ResponsavelId = p.ResponsavelId
            }).ToList();

            usuario.Areas = query1.Select(p => new Area
            {
                Id = p.Id,
                Nome = p.Nome,
                DepartamentoId = p.DepartamentoId
            }).ToList();

            ViewBag.Departamentos = _context.Departamentos.OrderBy(p => p.Nome).ToList();
            ViewBag.Areas = new SelectList(query1);

            return View(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "Gerenciador, Responsavel")]
        public async Task<IActionResult> Edit(UsuarioEditViewModel viewModel)
        {
            //if (!ModelState.IsValid)
            //    return View(viewModel);

            var identityUser = await userManager.GetUserAsync(User);
            var usuarioLogado = await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.Email == identityUser.Email);

            var usuarioEditar = await _context.Usuarios
                .Include(p => p.Departamento)
                .Include(p => p.Area)
                .FirstOrDefaultAsync(usuario => usuario.Id == viewModel.Id);

            bool isResponsavel = await userManager.IsInRoleAsync(identityUser, "Responsavel");
            bool mesmoDepartamento = usuarioEditar.DepartamentoId == usuarioLogado.DepartamentoId;

            if (isResponsavel && !mesmoDepartamento)
            {
                ViewBag.Error = "Não pode alterar usuario de outro departamento";
                return RedirectToAction(nameof(Edit));
            }

            usuarioEditar.Nome = viewModel.Nome;
            usuarioEditar.Cargo = viewModel.Cargo;
            usuarioEditar.DepartamentoId = viewModel.DepartamentoId;
            usuarioEditar.AreaId = viewModel.AreaId;

            await _context.SaveChangesAsync();

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
