using DAL.Dtos;
using DAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Models.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


public struct Login
{
    public string mail { get; set; }
    public string password { get; set; }

    public Login(string Mail, string Password)
    {
        mail = Mail;
        password = Password;
    }
}
public struct Register
{
    public string mail { get; set; }
    public string password { get; set; }
    public string name { get; set; }

    public Register(string Mail, string Password, string Name)
    {
        mail = Mail;
        password = Password;
        name = Name;
    }
}
namespace Groups.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : Controller
    {

        private IConfiguration _config;
        private readonly IUser _user;


        public LogInController(IConfiguration config, IUser user)
        {
            _config = config;
            _user = user;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Login loginRequest)
        {
            UserDto userFind = await _user.getUserByEmail(loginRequest.mail);


            if (userFind == null)
            {
                return BadRequest("User not found");
            }

            if (userFind.Password == loginRequest.password)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                string role = "User";

                var claims = new[]
                {
                   new Claim(JwtRegisteredClaimNames.Sub, loginRequest.mail),
                   new Claim("id", userFind.Id.ToString()), // הוספת ה-Claim של id
                   new Claim("role", role)
                 };
                var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                return Ok(token);
            }
            return BadRequest("Invalid password");
        }

        [HttpPost("SignIn/")]
        public async Task<IActionResult> Post([FromBody] UserDto signinRequest)
        {
            UserDto userFind = await _user.getUserByEmail(signinRequest.Email);


            if (userFind == null)
            {
                bool ans = await _user.createUser(signinRequest);
                if (!ans)
                    return BadRequest();
                Login loginReq = new Login(signinRequest.Email, signinRequest.Password);
                return await Post(loginReq);
            }

            return BadRequest("The user already exists");
        }
    }
}