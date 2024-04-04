using AutoMapper;
using Sereno.Identity.Entities.DataTransferObjects;
using Sereno.Identity.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sereno.Identity.JwtFeatures;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Sereno.Communication.Email;

namespace Sereno.Identity.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<User> userManager, IMapper mapper, IEmailSender emailSender, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _emailSender = emailSender;
        }


        [HttpGet("Privacy")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Privacy()
        {
            var claims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Ok(claims);
        }

        [HttpGet("Email")]
        public async Task<IActionResult> SendEmail()
        {
            var message = new Message(new string[] { "gerhard.allgaeuer@gmail.com" }, "Test email", "This is the content from our email.");
            await _emailSender.SendEmailAsync(message); 

            return StatusCode(200);
        }


        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
                return BadRequest();

            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password!);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }

            await _userManager.AddToRoleAsync(user, "Viewer");

            return StatusCode(201);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userForAuthentication.Email!);
                if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

                var signingCredentials = _jwtHandler.GetSigningCredentials();
                var claims = await _jwtHandler.GetClaims(user);
                var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
            }
            catch
            {
                throw;
            }

        }
    }
}
