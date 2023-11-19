using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using Tickest.Services.Authentication;

namespace Tickest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly TickestContext _context;

        private readonly IAuthenticationService _authenticationService;

        public AccountController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager, TickestContext context,
            IAuthenticationService authenticationService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _authenticationService = authenticationService;
        }

        [Authorize(Policy = "AdminGerenciadorPolicy")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            //if (!ModelState.IsValid)
            //    return View(registerModel);

            var registerResult = await _authenticationService.RegisterAsync(registerModel);
            if (!registerResult.Success)
            {
                foreach (var keyError in registerResult.Errors)
                    ModelState.AddModelError(keyError.Key, keyError.Error);

                return View(registerModel);
            }

            return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });


            //if (ModelState.IsValid)
            //{
            //    // Copia os dados do RegisterViewModel para o IdentityUser
            //    var user = new Usuario
            //    {
            //        UserName = registerModel.Nome,
            //        Email = registerModel.Email,
            //        NormalizedUserName = registerModel.Nome.ToUpper(),
            //        NormalizedEmail = registerModel.Email.ToUpper(),
            //    };

            //    // Armazena os dados do usuário na tabela AspNetUsers
            //    var result = await userManager.CreateAsync(user, registerModel.Senha);

            //    // Se o usuário foi criado com sucesso, faz o login do usuário usando o serviço
            //    // SignInManager e redireciona para o método Action Index
            //    if (result.Succeeded)
            //    {
            //        if (registerModel.Gerenciador)
            //            await userManager.AddToRoleAsync(user, "Gerenciador");
            //        else if (registerModel.Responsavel)
            //            await userManager.AddToRoleAsync(user, "Responsavel");
            //        else
            //            await userManager.AddToRoleAsync(user, "Colaborador");

            //        var usuario = new Usuario()
            //        {
            //            Nome = registerModel.Nome,
            //            Email = registerModel.Email,
            //            CargoId = 1,
            //            DepartamentoId = 1
            //        };

            //        _context.Add(usuario);
            //        await _context.SaveChangesAsync();
            //    }

            //    // Se houver erros, inclui no ModelState e exibe pela tag helper summary na validação
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            //}
            //return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
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
            if (!ModelState.IsValid)
                return View(loginModel);

            var loginResult = await _authenticationService.LoginAsync(loginModel);
            if (!loginResult.Success)
            {
                foreach (var keyError in loginResult.Errors)
                    ModelState.AddModelError(keyError.Key, keyError.Error);

                return View(loginModel);
            }

            var user = await userManager.FindByEmailAsync(loginModel.Email);

            if (await userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Index", "Admin", new { area = "Admin" });

            if (await userManager.IsInRoleAsync(user, "Gerenciador"))
                return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });

            if (await userManager.IsInRoleAsync(user, "Responsavel"))
                return RedirectToAction("Index", "Responsavel", new { area = "Responsavel" });

            return RedirectToAction("Index", "Colaboradores");
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
