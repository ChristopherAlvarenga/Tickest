using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{

    public class ChatController : Controller
    {
		private readonly TickestContext _context;

		public ChatController(TickestContext context) 
        {
			_context = context;
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

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public JsonResult Enviar(int ticket_id, string msg)
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
