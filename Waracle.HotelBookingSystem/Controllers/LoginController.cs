using Microsoft.AspNetCore.Mvc;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.IdentityModel.Tokens;

    [Authorize]
    [ApiController]
    [Route("/api/login")]
    [ApiVersion("1.0")]
    public class LoginController : BaseController
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
                             {
                                 new Claim(ClaimTypes.Name, "Don Chelladurai"),
                                 new Claim(JwtRegisteredClaimNames.Sub, "donchelladurai@gmail.com"),
                                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                             };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return this.Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // Models/TokenResponse.cs
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
