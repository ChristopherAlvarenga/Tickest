using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;

namespace Tickest.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TickestContext _context;

        public SeedUserRoleInitial(UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager,
            TickestContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SeedCargoAsync()
        {
            var cargos = new List<Cargo>
            {
                new Cargo{ Nome = "Analista Suporte" },
                new Cargo{ Nome = "Sistemas" },
            };

            foreach (var cargo in cargos)
            {
                var existeCargo = _context.Cargos.Any(p => p.Nome == cargo.Nome);
                if (!existeCargo)
                    await _context.Cargos.AddAsync(cargo);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Gerenciador"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Gerenciador";
                role.NormalizedName = "GERENCIADOR";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Responsavel"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Responsavel";
                role.NormalizedName = "RESPONSAVEL";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Colaborador"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Colaborador";
                role.NormalizedName = "COLABORADOR";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
        }

        public async Task SeedUsersAsync()
        {
            if (await _userManager.FindByEmailAsync("admin@localhost") == null)
            {
                Usuario user = new Usuario();
                user.UserName = "admin@localhost";
                user.Email = "admin@localhost";
                user.NormalizedUserName = "ADMIN@LOCALHOST";
                user.NormalizedEmail = "ADMIN@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            if (await _userManager.FindByEmailAsync("gerenciador@localhost") == null)
            {
                Usuario user = new Usuario();
                user.UserName = "gerenciador@localhost";
                user.Email = "gerenciador@localhost";
                user.NormalizedUserName = "GERENCIADOR@LOCALHOST";
                user.NormalizedEmail = "GERENCIADOR@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Gerenciador");
                }
            }

            if (await _userManager.FindByEmailAsync("responsavel@localhost") == null)
            {
                Usuario user = new Usuario();
                user.UserName = "responsavel@localhost";
                user.Email = "responsavel@localhost";
                user.NormalizedUserName = "RESPONSAVEL@LOCALHOST";
                user.NormalizedEmail = "RESPONSAVEL@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Responsavel");
                }
            }

            if (await _userManager.FindByEmailAsync("colaborador@localhost") == null)
            {
                Usuario user = new Usuario();
                user.UserName = "colaborador@localhost";
                user.Email = "colaborador@localhost";
                user.NormalizedUserName = "COLABORADOR@LOCALHOST";
                user.NormalizedEmail = "COLABORADOR@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Colaborador");
                }
            }
        }
    }
}
