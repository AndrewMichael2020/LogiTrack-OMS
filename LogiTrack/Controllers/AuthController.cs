using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LogiTrack.Models;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { message = "Username, email, and password are required." });

            // Enforce strong password policy
            if (model.Password.Length < 8 || !model.Password.Any(char.IsDigit) || !model.Password.Any(char.IsUpper))
                return BadRequest(new { message = "Password must be at least 8 characters, include a digit and an uppercase letter." });

            // Prevent duplicate usernames/emails
            if (await _userManager.FindByNameAsync(model.Username) != null)
                return BadRequest(new { message = "Username already exists." });
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Email already registered." });

            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(new { message = "Registration successful." });

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid username or password." });

            // Generate JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "supersecretkey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "logitrack",
                audience: _config["Jwt:Audience"] ?? "logitrack",
                claims: claims,
                expires: System.DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
