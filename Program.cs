using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Api.Data;
using UserManagement.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// 2. Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    // Opciones de contraseña:
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. JWT Authentication setup
var jwtConfig = builder.Configuration.GetSection("JwtSettings");
var keyBytes = Encoding.UTF8.GetBytes(jwtConfig["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtConfig["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:3000")  // tu frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ─── Seed de Roles y Admin ───
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1) Asegurar roles "Admin" y "User"
    var roles = new[] { "Admin", "User" };
    foreach (var rol in roles)
    {
        if (!await roleMgr.RoleExistsAsync(rol))
            await roleMgr.CreateAsync(new IdentityRole<int>(rol));
    }

    // 2) Crear un usuario "admin" si no existe
    const string adminUserName = "admin";
    const string adminEmail = "admin@tuempresa.com";
    const string adminPass = "Admin123!";

    if (await userMgr.FindByNameAsync(adminUserName) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            FirstName = "Administrador",
            LastName = "Sistema",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = string.Empty
        };
        var createResult = await userMgr.CreateAsync(admin, adminPass);
        if (createResult.Succeeded)
            await userMgr.AddToRoleAsync(admin, "Admin");
        // (Opcional: manejar errores de createResult.Errors)
    }
}
// ──────────────────────────────

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors();
app.UseAuthentication();   // <- Muy importante que vaya antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }