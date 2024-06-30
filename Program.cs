using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<TickestContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Usuario, Role>()
    .AddEntityFrameworkStores<TickestContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AspNetCore.Cookies";
        options.ExpireTimeSpan = TimeSpan.FromHours(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("EditPolicy", policy => policy.RequireRole("Gerenciador"));
//});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditPolicy", policy =>
    {
        policy.RequireRole("Gerenciador", "Analista");
    });
});


builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

await CreateAndSeedRolesAndUsersAsync(app);

app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

async Task CreateAndSeedRolesAndUsersAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<ISeedUserRoleInitial>();
    var context = scope.ServiceProvider.GetRequiredService<TickestContext>();

    await context.Database.MigrateAsync();
    await service.SeedRolesAsync();
    await service.SeedUsersAsync();
}
