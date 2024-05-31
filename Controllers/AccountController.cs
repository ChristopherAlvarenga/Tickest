using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly TickestContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, TickestContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Departamentos = _context.Departamentos.ToList();
            return View();
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                // Copia os dados do RegisterViewModel para o IdentityUser
                var user = new IdentityUser
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email,
                    NormalizedUserName = registerModel.Nome.ToUpper(),
                    NormalizedEmail = registerModel.Email.ToUpper()
                };

                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, registerModel.Senha);

                // Se o usuário foi criado com sucesso, faz o login do usuário usando o serviço
                // SignInManager e redireciona para o método Action Index
                if (result.Succeeded)
                {
                    if (registerModel.Funcao == 1)
                        await userManager.AddToRoleAsync(user, "Gerenciador");
                    else if (registerModel.Funcao == 2)
                        await userManager.AddToRoleAsync(user, "Responsavel");
                    else if (registerModel.Funcao == 3)
                        await userManager.AddToRoleAsync(user, "Desenvolvedor");

                    var usuario = new Usuario()
                    {
                        Nome = registerModel.Nome,
                        Email = registerModel.Email
                    };

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Gerenciador");
                }

                // Se houver erros, inclui no ModelState e exibe pela tag helper summary na validação
                if (result.Errors != null)
                {
                    ModelState.AddModelError(string.Empty, "A senha deve possuir mais de 6 caracteres.");
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos um caractere especial ('@','#', '&', etc).");
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos uma letra minúscula ('a'-'z').");
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos uma letra maiúscula ('A'-'Z').");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    loginModel.Email, loginModel.Senha, loginModel.ManterLogin, false);

                if (result.Succeeded)
                {

                    var user = await userManager.FindByEmailAsync(loginModel.Email);

                    if (await userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });

                    else if (await userManager.IsInRoleAsync(user, "Gerenciador"))
                        return RedirectToAction("Index", "Gerenciador");

                    else if (await userManager.IsInRoleAsync(user, "Responsavel"))
                        return RedirectToAction("Index", "Responsaveis");

                    else
                        return RedirectToAction("Index", "Desenvolvedores");

                }

                ModelState.AddModelError(string.Empty, "Login Inválido");
                return View(loginModel);
            }
            ModelState.AddModelError(string.Empty, "Login Inválido");
            return View(loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost("API/App/Login")]
        public async Task<IActionResult> Login_Mobile([FromBody] LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    loginModel.Email, loginModel.Senha, loginModel.ManterLogin, false);

                if (result.Succeeded)
                {

                    var user = await userManager.FindByEmailAsync(loginModel.Email);

                    var userInfo = await _context.Usuarios
                        .Include(p => p.Departamento)
                        .Include(p => p.Area)
                        .Where(p => p.Email == user.Email).FirstOrDefaultAsync();

                    var userTicketsCriados = await _context.Tickets
                        .Where(p => p.UsuarioId == userInfo.Id)
                        .Select(p => new
                            {
                                Id = p.Id,
                                Título = p.Título,
                                Descrição = p.Descrição,
                                Data_Criação = p.Data_Criação,
                                Status = p.Status,
                                Data_Status = p.Data_Status,
                                Prioridade = p.Prioridade,
                                Usuario = p.UsuarioId,
                                Departamento = p.Departamento.Nome,
                                DestinatarioId = p.DestinatarioId,
                                Anexos = p.Anexos

                            })
                        .ToListAsync();

                    var userTicketsRecebidos = await _context.Tickets
                        .Where(p => p.DestinatarioId == userInfo.Id || (p.DestinatarioId == null && p.UsuarioId != userInfo.Id))
                        .Select(p => new
                        {
                            Id = p.Id,
                            Título = p.Título,
                            Descrição = p.Descrição,
                            Data_Criação = p.Data_Criação,
                            Status = p.Status,
                            Data_Status = p.Data_Status,
                            Prioridade = p.Prioridade,
                            Usuario = p.UsuarioId,
                            Departamento = p.Departamento.Nome,
                            DestinatarioId = p.DestinatarioId,
                            Anexos = p.Anexos

                        })
                        .ToListAsync();

                    var userInfoApp = new
                    {
                        Id = userInfo.Id,
                        Nome = userInfo.Nome,
                        Email = userInfo.Email,
                        Area = userInfo.Area.Nome,
                        Departamento = userInfo.Departamento.Nome,
                        Cargo = userInfo.Cargo,
                        TicketsCriados = userTicketsCriados,
                        TicketsRecebidos = userTicketsRecebidos
                    };

                    return Ok(userInfoApp);

                }

                return BadRequest("Login Inválido");
            }
            return BadRequest("Login Inválido");
        }
    }
}
