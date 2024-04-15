using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Tickest.Controllers;
using Tickest.Models.Entities;

namespace Tickest.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public SeedUserRoleInitial(UserManager<Usuario> userManager,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Gerenciador"))
            {
                Role role = new Role();
                role.Name = "Gerenciador";
                role.NormalizedName = "GERENCIADOR";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Analista"))
            {
                Role role = new Role();
                role.Name = "Analista";
                role.NormalizedName = "Analista";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("cliente"))
            {
                Role role = new Role();
                role.Name = "Cliente";
                role.NormalizedName = "CLIENTE";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
        }

        public async Task SeedUsersAsync()
        {
            if (await _userManager.FindByEmailAsync("gerenciador@localhost") == null)
            {
                Usuario user = new Usuario();
                user.Nome = "Gerenciador";
                user.UserName = "gerenciador@localhost";
                user.Email = "gerenciador@localhost";
                user.NormalizedUserName = "GERENCIADOR@LOCALHOST";
                user.NormalizedEmail = "GERENCIADOR@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();
                //user.Cargo = "Gerenciador";
                user.DepartamentoId = 1;
                user.EspecialidadeId = 5;

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Gerenciador");
                }
            }

         
            if (await _userManager.FindByEmailAsync("rafael@localhost") == null)
            {
                Usuario user = new Usuario();
                user.Nome = string.Empty;
                user.UserName = "rafael@localhost";
                user.Email = "rafael@localhost";
                user.NormalizedUserName = "RAFAEL@LOCALHOST";
                user.NormalizedEmail = "RAFAEL@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();
                //user.Cargo = "Analista";
                user.DepartamentoId = 1;
                user.EspecialidadeId = 1;

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Cliente");
                }
            }

            if (await _userManager.FindByEmailAsync("analista@localhost") == null)
            {
                Usuario user = new Usuario();
                user.Nome = string.Empty;
                user.UserName = "analista@localhost";
                user.Email = "analista@localhost";
                user.NormalizedUserName = "ANALISTA@LOCALHOST";
                user.NormalizedEmail = "ANALISTA@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();
                //user.Cargo = "Analista";
                user.DepartamentoId = 1;
                user.EspecialidadeId = 1;

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Analista");
                }
            }
        }
    }
}
