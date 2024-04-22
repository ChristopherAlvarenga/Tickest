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
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public TicketsController(UserManager<Usuario> userManager, TickestContext context)
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
                .Include(p => p.Responsavel)
                .Include(p => p.Anexos)
                .Where(p => p.Responsavel.Email == usuario.Email)
                .Where(p => p.Status != Ticket.Tipo.Concluido && p.Status != Ticket.Tipo.Cancelado)
                .OrderBy(p => p.Id)
                .AsQueryable()
                .ToList();

            var viewModel = new TicketViewModel()
            {
                Tickets = query
                .Select(p => new Ticket
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Descricao = p.Descricao,
                    DataCriacao = p.DataCriacao,
                    Status = p.Status,
                    DataStatus = p.DataStatus,
                    Prioridade = p.Prioridade,
                    Responsavel = p.Responsavel,
                    Departamento = p.Departamento,
                    SolicitanteId = p.SolicitanteId,
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
                .Include(p => p.Responsavel)
                .Include(p => p.Anexos)
                .Where(p => p.Responsavel.Email == usuario.Email || p.SolicitanteId == usuario.Id)
                .Where(p => p.Status == Ticket.Tipo.Concluido || p.Status == Ticket.Tipo.Cancelado)
                .OrderBy(p => p.Id)
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Tickets = query.Select(p => new Ticket
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Descricao = p.Descricao,
                    DataCriacao = p.DataCriacao,
                    Status = p.Status,
                    DataStatus = p.DataStatus,
                    Prioridade = p.Prioridade,
                    Responsavel = p.Responsavel,
                    Departamento = p.Departamento,
                    SolicitanteId = p.SolicitanteId,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            return View(viewModel);
        }

        public IActionResult Search(string search)
        {
            var usuario = _context.Usuarios
               .Where(p => p.Email == User.Identity.Name)
               .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Responsavel)
                .Include(p => p.Anexos)
                .Where(p => p.Responsavel.Email == usuario.Email && (EF.Functions.Like(p.Titulo, "%" + search + "%") || (p.Id.ToString() == search) || (EF.Functions.Like(p.Descricao, "%" + search + "%") || (EF.Functions.Like(p.Departamento.Nome, "%" + search + "%")))))
                .OrderBy(p => p.Id)
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Tickets = query.Select(p => new Ticket
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    Descricao = p.Descricao,
                    DataCriacao = p.DataCriacao,
                    Status = p.Status,
                    DataStatus = p.DataStatus,
                    Prioridade = p.Prioridade,
                    Responsavel = p.Responsavel,
                    Departamento = p.Departamento,
                    SolicitanteId = p.SolicitanteId,
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
            var user = _context.Usuarios
                .Include(p => p.Departamento)
                .Where(p => p.Email == User.Identity.Name)
                .FirstOrDefault();

            var query = _context.Departamentos
                .OrderBy(p => p.Nome)
                .AsQueryable();

            var query1 = _context.Especialidades
                .OrderBy(p => p.Nome)
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Departamentos = query.Select(p => new Departamento
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    GerenciadorId = p.GerenciadorId
                }).ToList(),
                Especialidades = query1.Select(p => new Especialidade
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    DepartamentoId = p.DepartamentoId
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: TicketsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket, int departamentoId, List<IFormFile> files)
        {
            ViewBag.Especialidades = _context.Especialidades.Where(s => s.DepartamentoId == departamentoId);
            
            var usuario = _context.Usuarios
                .FirstOrDefault(p => p.Email == User.Identity.Name);

            foreach (IFormFile file in files)
            {
                var path = WriteFile(file);
                var fileName = Path.GetFileName(path);
                var name = "anexos/" + fileName;
                Anexo anexo = new Anexo();
                anexo.Endereco = name;
                ticket.Anexos.Add(anexo);
            }
            ticket.DataCriacao = DateTime.Now;
            ticket.Status = Ticket.Tipo.Criado;
            ticket.DataStatus = DateTime.Now;
            ticket.ResponsavelId = usuario.Id;
            ticket.DepartamentoId = ticket.DepartamentoId;
            _context.Add(ticket);
            
             await _context.SaveChangesAsync();
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

        public async Task<IActionResult> MudarStatus(int? id, [FromQuery(Name = "status")] int? status)
        {
            var usuario = _context.Usuarios
                .Where(u => u.Email == User.Identity.Name)
                .FirstOrDefault();

            var ticket = _context.Tickets
                .Where(t => t.Id == id)
                .FirstOrDefault();

            if (status == 1)
            {
                ticket.SolicitanteId = usuario.Id;
                ticket.Status = Ticket.Tipo.Andamento;
                ticket.DataStatus = DateTime.Now;
            }
            else if (status == 2)
            {
                ticket.Status = Ticket.Tipo.Teste;
                ticket.DataStatus = DateTime.Now;
            }

            else if (status == 3)
            {
                ticket.Status = Ticket.Tipo.Concluido;
                ticket.DataStatus = DateTime.Now;
            }

            else if (status == 4)
            {
                ticket.Status = Ticket.Tipo.Cancelado;
                ticket.DataStatus = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            if (await userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Index", "Admin", new { area = "Admin" });

            if (await userManager.IsInRoleAsync(user, "Gerenciador"))
                return RedirectToAction("Index", "Gerenciador");

            else if (await userManager.IsInRoleAsync(user, "Responsavel"))
                return RedirectToAction("Index", "Responsaveis");

            else
                return RedirectToAction("Index", "Desenvolvedores");
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }

}
