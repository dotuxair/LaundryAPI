using FYP.API.Data;
using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FYP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LaundaryDbContext _dbContext;
        private readonly Custom _methods;
        public AuthController(LaundaryDbContext context, Custom methods)
        {
            _dbContext = context;
            _methods = methods;

        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == request.Email && a.Password == request.Password);

                    if (user == null)
                    {
                        return NotFound(new { Error = "Wrong Email / Password" });
                    }
                   
                    var admin = await _dbContext.Admins.SingleOrDefaultAsync(a => a.UserId == user.Id);
                    var claims = new TokenDto()
                    {
                        Email = user.Email,
                    };
                    if (admin == null)
                    {
                        claims.Role = "User";
                        return Ok(new
                        {
                            Token = _methods.CreateToken(claims),
                            Role = "User",
                        });
                    }
                    else
                    {
                        claims.Role = "Admin";
                        return Ok(new
                        {
                            Token = _methods.CreateToken(claims),
                            Role = "Admin",
                        });
                    }
                }
                return BadRequest(new { Error = "Email and Password Feilds can't be empty." });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userexist = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == request.Email.ToLower());
                    if (userexist != null)
                    {
                        return NotFound(new { Error = "Email Already Exist." });
                    }

                    var user = new User()
                    {
                        Name = request.Name,
                        Email = request.Email.ToLower(),
                        Password = request.Password,
                        PhoneNumber = request.PhoneNumber,
                    };
                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                    return Ok($"Account Created with Name : " + user.Name);
                }
                return BadRequest(new {Error = ModelState});
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword request)
        {
            try
            {
                var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound(new { Error = "Email not found" });
                }
                user.Password = request.Password;
                await _dbContext.SaveChangesAsync();
                return Ok($"Password Changed Successfully");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            try
            {
                if (email.IsNullOrEmpty())
                {
                    return BadRequest(new { Error = "Email can't be empty." });
                }
                var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "Email does not exist" });
                }
                return Ok(new { Success = "Email Found" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
