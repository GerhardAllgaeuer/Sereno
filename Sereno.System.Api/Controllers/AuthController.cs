using Sereno.Logging;
using Sereno.System.Api.Config;
using Sereno.System.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Sereno.System.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppConfig config;
        private readonly UserService userService;

        public AuthController(AppConfig config, UserService userService)
        {
            this.config = config;
            this.userService = userService;
        }

        [Route("api/v1/[controller]/[action]")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            Context? context = HttpContext.Items["Context"] as Context;

            try
            {
                if (!userService.VerifyUser(login.Username, login.Password))
                {
                    Log.Instance.Error(context, $"Login Error {login.Username}");
                    return Unauthorized("Wrong credentials");
                }

                var now = DateTime.UtcNow;
                var expires = now.Add(config.IdentitySettings.TokenLifeTime);
                var issuer = config.IdentitySettings.Issuer;
                var audience = config.IdentitySettings.Audience;

                var claims = new[]
                {
                    new Claim("name", login.Username),
                    new Claim("role", "Documents"),
                    new Claim(JwtRegisteredClaimNames.Iat,
                              new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                              ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Nbf,
                              new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                              ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Exp,
                              new DateTimeOffset(expires).ToUnixTimeSeconds().ToString(),
                              ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Iss, issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, audience)
                };

                SymmetricSecurityKey key = config.IdentitySettings.GetSecurityKey();
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore: now,
                    expires: expires,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                Log.Instance.Info(context, $"User {login.Username} successfully logged in");
                return Ok(new { token = jwt });
            }
            catch (Exception exception)
            {
                Log.Instance.Error(context, exception);
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }
    }

    public record LoginRequest(string Username, string Password);
}
