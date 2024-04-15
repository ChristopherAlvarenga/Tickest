using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly TickestContext _context;
        private readonly RoleManager<Role> _roleManager;

        public AccountController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager, TickestContext context,
            RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            //ViewBag.Departamentos = _context.Departamentos.ToList();

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new RegisterViewModel
            {
                OpcesFuncoes = roles.Select(p => new FuncaoViewModel { Id = p.Id, Nome = p.Name }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copia os dados do RegisterViewModel para o IdentityUser
                var user = new Usuario
                {
                    Nome = model.Nome,
                    UserName = model.Email,
                    Email = model.Email,
                    NormalizedUserName = model.Nome.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper()
                };

                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Senha);

                // Se o usuário foi criado com sucesso, faz o login do usuário usando o serviço
                // SignInManager e redireciona para o método Action Index
                if (result.Succeeded)
                {
                    var funcaoSelecionada = await _roleManager.FindByIdAsync(model.FuncaoId.ToString());

                    await userManager.AddToRoleAsync(user, funcaoSelecionada.Name);

                    //var usuario = new Usuario()
                    //{
                    //    Nome = model.Nome,
                    //    Email = model.Email
                    //};

                    //_context.Add(usuario);
                    //await _context.SaveChangesAsync();

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

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new RegisterViewModel
            {
                OpcesFuncoes = roles.Select(p => new FuncaoViewModel { Id = p.Id, Nome = p.Name }).ToList()
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
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
                        return RedirectToAction("Index", "Analista");

                }

                ModelState.AddModelError(string.Empty, "Login Inválido");
                return View(loginModel);
            }
            ModelState.AddModelError(string.Empty, "Login Inválido");
            return View(loginModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
