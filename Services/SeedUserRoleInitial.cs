using Microsoft.AspNetCore.Identity;
using Tickest.Models.Entities;

namespace Tickest.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public SeedUserRoleInitial(UserManager<Usuario> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Gerenciador"))
            {
                var role = new Role
                {
                    Name = "Gerenciador",
                    NormalizedName = "GERENCIADOR"
                };
                await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Analista"))
            {
                var role = new Role
                {
                    Name = "Analista",
                    NormalizedName = "ANALISTA"
                };
                await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Cliente"))
            {
                var role = new Role
                {
                    Name = "Cliente",
                    NormalizedName = "CLIENTE"
                };
                await _roleManager.CreateAsync(role);
            }
        }

        public async Task SeedUsersAsync()
        {
            if (await _userManager.FindByEmailAsync("gerenciador@localhost") == null)
            {
                var user = new Usuario
                {
                    Nome = "Gerenciador",
                    UserName = "gerenciador@localhost",
                    Email = "gerenciador@localhost",
                    NormalizedUserName = "GERENCIADOR@LOCALHOST",
                    NormalizedEmail = "GERENCIADOR@LOCALHOST",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    DepartamentoId = 1,
                    EspecialidadeId = 5
                };

                var result = await _userManager.CreateAsync(user, "#SecretPass456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Gerenciador");
                }
            }

            if (await _userManager.FindByEmailAsync("rafael@localhost") == null)
            {
                var user = new Usuario
                {
                    Nome = "Rafael",
                    UserName = "rafael@localhost",
                    Email = "rafael@localhost",
                    NormalizedUserName = "RAFAEL@LOCALHOST",
                    NormalizedEmail = "RAFAEL@LOCALHOST",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    DepartamentoId = 1,
                    EspecialidadeId = 1
                };

                var result = await _userManager.CreateAsync(user, "#SecretPass456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Cliente");
                }
            }

            if (await _userManager.FindByEmailAsync("analista@localhost") == null)
            {
                var user = new Usuario
                {
                    Nome = "Analista",
                    UserName = "analista@localhost",
                    Email = "analista@localhost",
                    NormalizedUserName = "ANALISTA@LOCALHOST",
                    NormalizedEmail = "ANALISTA@LOCALHOST",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    DepartamentoId = 1,
                    EspecialidadeId = 1
                };

                var result = await _userManager.CreateAsync(user, "#SecretPass456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Analista");
                }
            }
        }
    }
}
