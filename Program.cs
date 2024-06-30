using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<TickestContext>(o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TickestContext>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler =
	ReferenceHandler.IgnoreCycles;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AspNetCore.Cookies";
        options.ExpireTimeSpan = TimeSpan.FromHours(20);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

await CriarPerfilUsuarioAsync(app);

app.UseAuthentication();

app.UseAuthorization();

app.UseCookiePolicy();


app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
		name: "default",
	 pattern: "{controller=Account}/{action=Login}/{id?}");

	// Mapear o Hub do SignalR
	endpoints.MapHub<ChatHub>("/chatHub");
});
app.Run();

async Task CriarPerfilUsuarioAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>(); 

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        var context = scope.ServiceProvider.GetService<TickestContext>();

        context.Database.Migrate();

        await service.SeedRolesAsync();
        await service.SeedUsersAsync();
    }
}