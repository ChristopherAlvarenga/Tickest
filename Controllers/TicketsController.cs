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
                .Include(p => p.Anexos)
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
                    Departamento = p.Departamento,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Historic()
        {
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name)
                .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Usuario)
                .Include(p => p.Anexos)
                .Where(p => p.Usuario.Email == usuario.Email)
                .Where(p => p.DestinatarioId == usuario.Id)
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
                    Departamento = p.Departamento,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            return View(viewModel);
        }

        // GET: TicketsController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

       public IActionResult Search(string search)
        {
            var usuario = _context.Usuarios
               .Where(p => p.Email == User.Identity.Name)
               .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Usuario)
                .Include(p => p.Anexos)
                .Where(p => p.Usuario.Email == usuario.Email && (EF.Functions.Like(p.Título, "%" + search + "%") || (p.Id.ToString() == search) || (EF.Functions.Like(p.Descrição, "%" + search + "%") || (EF.Functions.Like(p.Departamento.Nome, "%" + search + "%")))))
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
                    Departamento = p.Departamento,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            ViewBag.titulo = "Pesquisando por " + search;

            return View(viewModel);

          
        }

        // GET: TicketsController/Create
        public IActionResult Create()
        {
            var query = _context.Departamentos
                .OrderBy(p => p.Nome)
                .AsQueryable();

            var query1 = _context.Areas
                .OrderBy(p => p.Nome)
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

            ViewBag.Departamentos = _context.Departamentos.OrderBy(p => p.Nome).ToList();
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
        public async Task<IActionResult> Create(Ticket ticket, int departamentoId, List<IFormFile> files)
        {
            ViewBag.Areas = _context.Areas.Where(s => s.DepartamentoId == departamentoId);
            ticket.Anexos = new List<Anexo>();
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name).FirstOrDefault();

            foreach (IFormFile file in files)
            {
                var path = WriteFile(file);
                var fileName = Path.GetFileName(path);
                var name = "anexos/" + fileName;
                Anexo anexo = new Anexo();
                anexo.Endereco = name;
                ticket.Anexos.Add(anexo);
            }
            ticket.Data_Criação = DateTime.Now;
            ticket.Status = Ticket.Tipo.Criado;
            ticket.Comentario = "";
            ticket.UsuarioId = usuario.Id;
            ticket.DepartamentoId = usuario.DepartamentoId;
            _context.Add(ticket);
            await _context.SaveChangesAsync();

            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            return RedirectToAction(nameof(Index));
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

        public static string WriteFile(IFormFile file)
        {
            string caminhoCompleto = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\anexos");

            if (!Directory.Exists(caminhoCompleto))
            {
                Directory.CreateDirectory(caminhoCompleto);
            }
            string path = caminhoCompleto + "\\" + GetTimestamp(DateTime.Now) + System.IO.Path.GetExtension(file.FileName);
            string name = Path.GetFileName(path);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return path;


        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
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
