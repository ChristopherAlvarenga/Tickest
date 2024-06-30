using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Plugins;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Message = Tickest.Models.Entities.Message;

namespace Tickest.Controllers
{

    public class ChatController : Controller
    {
		private readonly TickestContext _context;
		private readonly IHubContext<ChatHub> _hubContext;

		public ChatController(TickestContext context, IHubContext<ChatHub> hubContext) 
        {
			_context = context;
			_hubContext = hubContext;
		}


        public IActionResult Mensagens(int id)
        {
            int Id = 0;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account");
            }
            else
            {
                Usuario user = _context.Usuarios.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                Id = user.Id;
            }

                List<Message> messages = _context.Mensagens.Include(p => p.User_from).Where(x => x.ticket_id == id).ToList();

                ViewBag.Mensagens = messages;
                ViewBag.from = Id;
     
          
            return View();
        }

        public JsonResult Upload(IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var path = WriteFile(file);
                var fileName = Path.GetFileName(path);
                var name = "anexos/" + fileName;

                TempData["sucesso"] = name;
            }
            else
            {
                TempData["erro"] = "erro";

            }
            return Json(TempData);
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

        public static System.String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
		public async Task<JsonResult>Enviar(int ticket_id, string msg)
        {
            if (ModelState.IsValid)
            {
                Usuario user = _context.Usuarios.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                int id = user.Id;
                var usuario = _context.Usuarios.Where(x => x.Id == id).FirstOrDefault();
                Message mensagem = new Message();



				mensagem.user_id_from = id;
                mensagem.msg_content = msg;
                mensagem.dataHora = DateTime.Now;
                mensagem.visu_status = 0;
                mensagem.ticket_id = ticket_id;
				_context.Mensagens.Add(mensagem);
				_context.SaveChanges();
                TempData["sucesso"] = "sim";
				await _hubContext.Clients.Group("Group-" + ticket_id).SendAsync("ReceiveGroupMessage", new
				{
					From = usuario.Nome,
					From_id = usuario.Id,
					Message = msg,
					DateTime = mensagem.dataHora
				});
				return Json(TempData);

            }
            else
            {
                TempData["error"] = "Algum dado está incorreto ou faltando!";
                return Json(TempData);
            }
        }

		[HttpPost("enviarMobile")]
		public async Task<JsonResult> EnviarMobile([FromBody] VMMessage message)
		{
			if (ModelState.IsValid)
			{
				var id = message.from;
				var usuario = _context.Usuarios.Where(x => x.Id == id).FirstOrDefault();
				Message mensagem = new Message();



				mensagem.user_id_from = id;
				mensagem.msg_content = message.msg;
				mensagem.dataHora = DateTime.Now;
				mensagem.visu_status = 0;
				mensagem.ticket_id = message.ticket_id;
				_context.Mensagens.Add(mensagem);
				_context.SaveChanges();
				TempData["sucesso"] = "sim";
				await _hubContext.Clients.Group("Group-" + message.ticket_id).SendAsync("ReceiveGroupMessage", new
				{
					From = usuario.Nome,
					From_id = usuario.Id,
					Message = message.msg,
					DateTime = mensagem.dataHora
				});
				return Json(TempData);

			}
			else
			{
				TempData["error"] = "Algum dado está incorreto ou faltando!";
				return Json(TempData);
			}
		}
	}
}
