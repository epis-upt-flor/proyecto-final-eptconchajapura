using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using ProyectoCF.Models;
using ProyectoCF.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Configurar DbContext
builder.Services.AddDbContext<Connection>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 5, 26))
    )
);

// Configurar caché y sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar autenticación con Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
});

// Leer configuración de Telegram
var telegramConfig = builder.Configuration.GetSection("Telegram");
builder.Services.AddSingleton(new TelegramService(
    telegramConfig["BotToken"],
    telegramConfig["GroupId"]
));

var app = builder.Build();

// Configuración del entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}");

    endpoints.MapControllers();
});

app.Run();