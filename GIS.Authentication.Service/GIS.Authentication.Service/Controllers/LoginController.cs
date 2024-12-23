using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GIS.Country.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GIS.Country.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public LoginTokenDTO Login(LoginUserDTO loginUserDTO)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:SecretKey").Value));
            string issuer = _configuration.GetSection("JWT:Issuer").Value;
            string audience = _configuration.GetSection("JWT:Audience").Value;
            DateTime expiration = DateTime.UtcNow.AddDays(1);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, loginUserDTO.name),
                    new Claim(ClaimTypes.Email, loginUserDTO.email)
                }),
                Issuer = issuer,
                Audience = audience,
                Expires = expiration,
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new LoginTokenDTO(accessToken);
        }
        
    }
}
