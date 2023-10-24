using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TickestContext _context;

        public TicketsController(TickestContext context)
        {
            _context = context;
        }

        // GET: TicketsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TicketsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TicketsController/Create
        [Authorize]
        public IActionResult Create()
        {
            var query = _context.Departamentos.AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Departamentos = query.Select(p => new Departamento
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    ResponsavelId = p.ResponsavelId,
                    Areas = p.Areas
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: TicketsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Título,Descrição,Comentário,Data_Criação,Data_Limite,Prioridade,Status")] Ticket ticket)
        {
                ticket.Data_Criação = DateTime.Now;
                ticket.Status = Ticket.Tipo.Aberto;
                ticket.Comentario = "";
                _context.Add(ticket);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
        }

        // GET: TicketsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TicketsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TicketsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
