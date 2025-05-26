using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Api.Models;

namespace UserManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly IConfiguration _cfg;

        public AuthController(UserManager<ApplicationUser> userMgr, IConfiguration cfg)
        {
            _userMgr = userMgr;
            _cfg = cfg;
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                // Rellenar obligatorios:
                PhoneNumber = "",                      // no-nulo
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                // (si tienes más campos NOT NULL, asígnales valor aquí)
            };

            var result = await _userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 📌 Asignar rol “User” por defecto
            await _userMgr.AddToRoleAsync(user, "User");

            return Ok(new { message = "Usuario creado con éxito" });
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userMgr.FindByNameAsync(dto.Username);
            if (user == null || !await _userMgr.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Credenciales inválidas");

            // 1) Claims base
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // 2) Añade los roles como claims de tipo ClaimTypes.Role
            var roles = await _userMgr.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3) Genera el token con SigningCredentials
            var jwtConfig = _cfg.GetSection("JwtSettings");
            var keyBytes = Encoding.UTF8.GetBytes(jwtConfig["Key"]);
            var creds = new SigningCredentials(
                                new SymmetricSecurityKey(keyBytes),
                                SecurityAlgorithms.HmacSha256
                            );

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            });
        }
    }

    // DTOs para request bodies
    public record RegisterDto(string Username, string Email, string Password, string FirstName, string LastName);
    public record LoginDto(string Username, string Password);
}