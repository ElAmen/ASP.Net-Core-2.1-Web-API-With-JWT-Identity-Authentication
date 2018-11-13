using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private ApplicationDbContext context;
        
        public AuthController(UserManager<ApplicationUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            this.context = _context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            //return new string[] { "value1", "value2" };
            return context.Users.Select(u => u.UserName).ToArray();
        }

        //[HttpPost]
        //[Route("register")]

        //public async Task<IdentityResult> Register([FromBody] Register model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return null;
        //    }
        //    var user = new ApplicationUser()
        //    {
        //        UserName = model.UserName,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
        //        Position = model.Position,
        //        Organization = model.Organization,
        //        SenderName = model.SenderName
        //    };
        //    var result = await userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return null;
        //    }

        //    return result;
        //}


        [HttpPost]
        [Route("register")]

        public async Task<ActionResult<dynamic>> Register([FromBody] Register model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Position = model.Position,
                Organization = model.Organization,
                SenderName = model.SenderName
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(result);
            }


            return Ok(result);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super Secure Key"));

                var token = new JwtSecurityToken(
                    issuer: "http://cs.co",
                    audience: "http://cs.co",
                    expires: DateTime.UtcNow.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo

                    });
            }
            return Unauthorized();
        }
    }
}