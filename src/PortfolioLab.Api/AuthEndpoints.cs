using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioLab.Api;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app, string jwtKey)
    {
        app.MapPost("/register", async (RegisterRequest req, UserManager<IdentityUser> userManager) =>
        {
            var user = new IdentityUser { UserName = req.Email, Email = req.Email };
            var result = await userManager.CreateAsync(user, req.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors.Select(e => e.Description));
            }
            return Results.Created($"/users/{user.Id}", new { user.Id, user.Email });
        });

        app.MapPost("/login", async (LoginRequest req, UserManager<IdentityUser> UserManager, SignInManager<IdentityUser> SignInManager) =>
        {
            var user = await UserManager.FindByEmailAsync(req.Email);
            if (user is null)
                return Results.Unauthorized();

            var check = await SignInManager.CheckPasswordSignInAsync(user, req.Password, false);
            if (!check.Succeeded)
            {
                return Results.Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        });
    }
}