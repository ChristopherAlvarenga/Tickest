using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                .Where(p => p.Status != Ticket.Tipo.Concluído && p.Status != Ticket.Tipo.Cancelado)
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
                    Status = p.Status,
                    Data_Status = p.Data_Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento,
                    DestinatarioId = p.DestinatarioId,
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
                .Where(p => p.Usuario.Email == usuario.Email || p.DestinatarioId == usuario.Id)
                .Where(p => p.Status == Ticket.Tipo.Concluído || p.Status == Ticket.Tipo.Cancelado)
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
                    Status = p.Status,
                    Data_Status = p.Data_Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento,
                    DestinatarioId = p.DestinatarioId,
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
                    Status = p.Status,
                    Data_Status = p.Data_Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento,
                    DestinatarioId = p.DestinatarioId,
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

            ViewBag.Departamentos = _context.Departamentos.Where(p => p.Id != user.DepartamentoId).OrderBy(p => p.Nome).ToList();
            ViewBag.Areas = new SelectList(query1.Where(p => p.Id != user.AreaId));

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
            ticket.Data_Status = DateTime.Now;
            ticket.UsuarioId = usuario.Id;
            ticket.DepartamentoId = usuario.DepartamentoId;
            _context.Add(ticket);
            await _context.SaveChangesAsync();

            var user = await userManager.FindByEmailAsync(User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("api/mobile/createTicket")]
        public async Task<IActionResult> CreateTicketApp([FromBody] Ticket ticket, List<IFormFile> files)
        {
            ticket.Anexos = new List<Anexo>();
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name).FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ticket.Anexos != null && ticket.Anexos.Any())
            {
                foreach (IFormFile file in files)
                {
                    var path = WriteFile(file);
                    var fileName = Path.GetFileName(path);
                    var name = "anexos/" + fileName;
                    Anexo anexo = new Anexo();
                    anexo.Endereco = name;
                    ticket.Anexos.Add(anexo);
                }
            }

            ticket.Data_Criação = DateTime.Now;
            ticket.Status = Ticket.Tipo.Criado;
            ticket.Data_Status = DateTime.Now;
            ticket.UsuarioId = usuario.Id;
            ticket.DepartamentoId = usuario.DepartamentoId;
            _context.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket); // Retorna o ticket criado com sucesso
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
                ticket.DestinatarioId = usuario.Id;
                ticket.Status = Ticket.Tipo.Andamento;
                ticket.Data_Status = DateTime.Now;
            }
            else if (status == 2)
            {
                ticket.Status = Ticket.Tipo.Teste;
                ticket.Data_Status = DateTime.Now;
            }

            else if (status == 3)
            {
                ticket.Status = Ticket.Tipo.Concluído;
                ticket.Data_Status = DateTime.Now;
            }

            else if (status == 4)
            {
                ticket.Status = Ticket.Tipo.Cancelado;
                ticket.Data_Status = DateTime.Now;
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

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        [HttpGet("API/app/ticket/{ticket_id}")]
        public async Task<JsonResult> GetTicket(int ticket_id)
        {
            if (ticket_id > 0)
            {
                Ticket ticket = _context.Tickets.Where(p => p.Id == ticket_id).FirstOrDefault();

                if (ticket != null)
                {
                    ticket.status_nome = ticket.Status.ToString();
                    TempData["ticket"] = ticket;
                }
                else
                {
                    TempData["error"] = "ticket não encontrado!";
                }


            }
            else
            {
                TempData["error"] = "Ticket Inválido";
            }

            return Json(TempData);
        }

        [HttpGet("API/app/ticket/messages/{ticket_id}")]
        public async Task<JsonResult> GetTicketMessages(int ticket_id)
        {
            if (ticket_id > 0)
            {
                List<Message> messages = _context.Mensagens.Where(p => p.ticket_id == ticket_id).ToList();

                if (messages.Count > 0)
                {
                    TempData["messages"] = messages;

                }
                else
                {
                    TempData["no_messages"] = "yrds";
                }

            }
            return Json(TempData);
        }

        [HttpPost("api/mobile/home")]
        public IActionResult GetAllTickets([FromBody] FilterViewModel filter)
        {
            try
            {
                var search = 0;
                if (filter.StatusId != null)
                {
                    if (filter.searchText != null || filter.searchText != "")
                    {
                        var tickets = _context.Tickets
                            .Where(a => ((int)a.Status) == filter.StatusId)
                            .Where(a => EF.Functions.Like(a.Título, "%" + filter.searchText + "%") || EF.Functions.Like(a.Descrição, "%" + filter.searchText + "%"))
                            .Where(p => p.DestinatarioId == filter.userId || (p.DestinatarioId == null && p.UsuarioId != filter.userId))
                            .Where(p => p.Status != Ticket.Tipo.Concluído && p.Status != Ticket.Tipo.Cancelado)
                            .Select(a => new
                            {
                                Id = a.Id,
                                Título = a.Título,
                                Descrição = a.Descrição,
                                Data_Criação = a.Data_Criação,
                                Etapa = a.Status.ToString(),
                                Etapa_id = a.Status,
                                Area = a.Area.Nome,
                                Data_Status = a.Data_Status,
                                Prioridade = a.Prioridade,
                                Usuario = a.UsuarioId,
                                Departamento = a.Departamento.Nome,
                                DestinatarioId = a.DestinatarioId,
                                Anexos = a.Anexos
                            })
                        .ToList();
                        return Ok(tickets);
                    }
                    else
                    {
                        var tickets = _context.Tickets
                            .Where(a => ((int)a.Status) == filter.StatusId)
                            .Where(p => p.DestinatarioId == filter.userId || (p.DestinatarioId == null && p.UsuarioId != filter.userId))
                            .Where(p => p.Status != Ticket.Tipo.Concluído && p.Status != Ticket.Tipo.Cancelado)
                            .Select(a => new
                            {
                                Id = a.Id,
                                Título = a.Título,
                                Descrição = a.Descrição,
                                Data_Criação = a.Data_Criação,
                                Etapa = a.Status.ToString(),
                                Etapa_id = a.Status,
                                Area = a.Area.Nome,
                                Data_Status = a.Data_Status,
                                Prioridade = a.Prioridade,
                                Usuario = a.UsuarioId,
                                Departamento = a.Departamento.Nome,
                                DestinatarioId = a.DestinatarioId,
                                Anexos = a.Anexos
                            })
                        .ToList();
                        return Ok(tickets);
                    }

                }
                else
                {
                    if (filter.searchText != null || filter.searchText != "")
                    {
                        var tickets = _context.Tickets
                            .Where(p => p.DestinatarioId == filter.userId || (p.DestinatarioId == null && p.UsuarioId != filter.userId))
                            .Where(a => EF.Functions.Like(a.Título, "%" + filter.searchText + "%") || EF.Functions.Like(a.Descrição, "%" + filter.searchText + "%"))
                            .Where(p => p.Status != Ticket.Tipo.Concluído && p.Status != Ticket.Tipo.Cancelado)
                            .Select(a => new
                            {
                                Id = a.Id,
                                Título = a.Título,
                                Descrição = a.Descrição,
                                Data_Criação = a.Data_Criação,
                                Etapa = a.Status.ToString(),
                                Etapa_id = a.Status,
                                Area = a.Area.Nome,
                                Data_Status = a.Data_Status,
                                Prioridade = a.Prioridade,
                                Usuario = a.UsuarioId,
                                Departamento = a.Departamento.Nome,
                                DestinatarioId = a.DestinatarioId,
                                Anexos = a.Anexos
                            })
                        .ToList();
                        return Ok(tickets);
                    }
                    else
                    {
                        var tickets = _context.Tickets
                        .Where(p => p.DestinatarioId == filter.userId || (p.DestinatarioId == null && p.UsuarioId != filter.userId))
                        .Where(p => p.Status != Ticket.Tipo.Concluído && p.Status != Ticket.Tipo.Cancelado)
                        .Select(a => new
                        {
                            Id = a.Id,
                            Título = a.Título,
                            Descrição = a.Descrição,
                            Data_Criação = a.Data_Criação,
                            Etapa = a.Status.ToString(),
                            Etapa_id = a.Status,
                            Area = a.Area.Nome,
                            Data_Status = a.Data_Status,
                            Prioridade = a.Prioridade,
                            Usuario = a.UsuarioId,
                            Departamento = a.Departamento.Nome,
                            DestinatarioId = a.DestinatarioId,
                            Anexos = a.Anexos
                        })
                        .ToList();
                        return Ok(tickets);
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost("api/mobile/historico")]
        public IActionResult GetTicketsHitoric([FromBody] FilterViewModel filter)
        {
            try
            {
                var search = 0;
                if (filter.StatusId != null)
                {
                    if (filter.searchText != null || filter.searchText != "")
                    {
                        var tickets = _context.Tickets
                            .Where(a => ((int)a.Status) == filter.StatusId)
                            .Where(a => EF.Functions.Like(a.Título, "%" + filter.searchText + "%") || EF.Functions.Like(a.Descrição, "%" + filter.searchText + "%"))
                            .Where(p => p.DestinatarioId == filter.userId)
                            .Where(p => p.Status == Ticket.Tipo.Concluído || p.Status == Ticket.Tipo.Cancelado)
                            .Select(a => new
                            {
                                Id = a.Id,
                                Título = a.Título,
                                Descrição = a.Descrição,
                                Data_Criação = a.Data_Criação,
                                Etapa = a.Status.ToString(),
                                Etapa_id = a.Status,
                                Area = a.Area.Nome,
                                Data_Status = a.Data_Status,
                                Prioridade = a.Prioridade,
                                Usuario = a.UsuarioId,
                                Departamento = a.Departamento.Nome,
                                DestinatarioId = a.DestinatarioId,
                                Anexos = a.Anexos
                            })
                        .ToList();
                        return Ok(tickets);
                    }
                    else
                    {
                        var tickets = _context.Tickets
                            .Where(a => ((int)a.Status) == filter.StatusId)
                            .Where(p => p.DestinatarioId == filter.userId)
                            .Where(p => p.Status == Ticket.Tipo.Concluído || p.Status == Ticket.Tipo.Cancelado)
                            .Select(a => new
                            {
                                Id = a.Id,
                                Título = a.Título,
                                Descrição = a.Descrição,
                                Data_Criação = a.Data_Criação,
                                Etapa = a.Status.ToString(),
                                Etapa_id = a.Status,
                                Area = a.Area.Nome,
                                Data_Status = a.Data_Status,
                                Prioridade = a.Prioridade,
                                Usuario = a.UsuarioId,
                                Departamento = a.Departamento.Nome,
                                DestinatarioId = a.DestinatarioId,
                                Anexos = a.Anexos
                            })
                        .ToList();
                        return Ok(tickets);
                    }

                }
                else
                {
                    if (filter.searchText != null || filter.searchText != "")
                    {
                        var tickets = _context.Tickets
                            .Where(p => p.DestinatarioId == filter.userId)
                            .Where(a => EF.Functions.Like(a.Título, "%" + filter.searchText + "%") || EF.Functions.Like(a.Descrição, "%" + filter.searchText + "%"))
                            .Where(p => p.Status == Ticket.Tipo.Concluído || p.Status == Ticket.Tipo.Cancelado)
                            .Select(a => new
                            {
                                Id = a.Id,
                                Título = a.Título,
                                Descrição = a.Descrição,
                                Data_Criação = a.Data_Criação,
                                Etapa = a.Status.ToString(),
                                Etapa_id = a.Status,
                                Area = a.Area.Nome,
                                Data_Status = a.Data_Status,
                                Prioridade = a.Prioridade,
                                Usuario = a.UsuarioId,
                                Departamento = a.Departamento.Nome,
                                DestinatarioId = a.DestinatarioId,
                                Anexos = a.Anexos
                            })
                        .ToList();
                        return Ok(tickets);
                    }
                    else
                    {
                        var tickets = _context.Tickets
                        .Where(p => p.DestinatarioId == filter.userId)
                        .Where(p => p.Status == Ticket.Tipo.Concluído || p.Status == Ticket.Tipo.Cancelado)
                        .Select(a => new
                        {
                            Id = a.Id,
                            Título = a.Título,
                            Descrição = a.Descrição,
                            Data_Criação = a.Data_Criação,
                            Etapa = a.Status.ToString(),
                            Etapa_id = a.Status,
                            Area = a.Area.Nome,
                            Data_Status = a.Data_Status,
                            Prioridade = a.Prioridade,
                            Usuario = a.UsuarioId,
                            Departamento = a.Departamento.Nome,
                            DestinatarioId = a.DestinatarioId,
                            Anexos = a.Anexos
                        })
                        .ToList();
                        return Ok(tickets);
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

    }
}
