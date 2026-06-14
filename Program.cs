using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Render.com PORT desteği
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı: DATABASE_URL varsa PostgreSQL, yoksa SQL Server
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Render.com PostgreSQL bağlantısı
    var connStr = ConvertDatabaseUrl(databaseUrl);
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connStr));
}
else
{
    // Lokal geliştirme - SQL Server
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
if (!string.IsNullOrEmpty(googleAuthNSection["ClientId"]) && !string.IsNullOrEmpty(googleAuthNSection["ClientSecret"]))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleAuthNSection["ClientId"]!;
            options.ClientSecret = googleAuthNSection["ClientSecret"]!;
        });
}

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<IProjectService, ProjectManager>();

var app = builder.Build();

// Otomatik migration ve veritabanı oluşturma
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Render.com (PostgreSQL)
        // DİKKAT: Veritabanını sıfırlama isteği üzerine eklendi.
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
    else
    {
        // Lokal geliştirme (SQL Server) - Migration'ları uygula
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Render.com'da HTTPS redirect sorun yaratır, sadece lokalde kullan
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RENDER")))
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

// Render.com DATABASE_URL formatını .NET connection string'e çevir
static string ConvertDatabaseUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var portNum = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var username = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";

    return $"Host={host};Port={portNum};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}

