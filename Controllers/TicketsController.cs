using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly TickestContext _context;

        public TicketsController(UserManager<IdentityUser> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: TicketsController
        public IActionResult Index()
        {
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name)
                .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Usuario)
                .Where(p => p.Usuario.Email == usuario.Email)
                .OrderBy(p => p.Id)
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Tickets = query.Select(p => new Ticket
                {
                    Id = p.Id,
                    Título = p.Título,
                    Descrição = p.Descrição,
                    Data_Criação = p.Data_Criação,
                    Comentario = p.Comentario,
                    Status = p.Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento
                }).ToList(),
                Usuario = usuario
            };

            return View(viewModel);
        }

        // GET: TicketsController/Details/5
        public IActionResult Historic()
        {
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name)
                .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Usuario)
                .Where(p => p.Usuario.Email == usuario.Email)
                .Where(p => p.Status == Ticket.Tipo.Concluído)
                .Where(p => p.Status == Ticket.Tipo.Cancelado)
                .OrderBy(p => p.Id)
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Tickets = query.Select(p => new Ticket
                {
                    Id = p.Id,
                    Título = p.Título,
                    Descrição = p.Descrição,
                    Data_Criação = p.Data_Criação,
                    Comentario = p.Comentario,
                    Status = p.Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento
                }).ToList(),
                Usuario = usuario
            };

            return View(viewModel);
        }

        // GET: TicketsController/Create
        public IActionResult Create()
        {
            var query = _context.Departamentos
                .AsQueryable();

            var query1 = _context.Areas
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Departamentos = query.Select(p => new Departamento
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    ResponsavelId = p.ResponsavelId
                }).ToList(),
                Areas = query1.Select(p => new Area
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    DepartamentoId = p.DepartamentoId
                }).ToList()
            };

            ViewBag.Departamentos = _context.Departamentos.ToList();
            ViewBag.Areas = new SelectList(query1);

            return View(viewModel);
        }

        private object ToList()
        {
            throw new NotImplementedException();
        }

        // POST: TicketsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Título,Descrição,Comentário,Data_Criação,Data_Limite,Prioridade,Status,AreaId")] Ticket ticket, int departamentoId, IFormFile files)
        {
            ViewBag.Areas = _context.Areas.Where(s => s.DepartamentoId == departamentoId);

            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name).FirstOrDefault();

            ticket.Data_Criação = DateTime.Now;
            ticket.Status = Ticket.Tipo.Criado;
            ticket.Comentario = "";
            ticket.UsuarioId = usuario.Id;
            ticket.DepartamentoId = usuario.DepartamentoId;
            _context.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Desenvolvedores");
        }

        // GET: TicketsController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: TicketsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TicketsController/Delete/5
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: TicketsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
