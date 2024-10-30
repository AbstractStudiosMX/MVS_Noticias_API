using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DataService;
using MVS_Noticias_API.Models.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MVS_Noticias_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthenticationController(IConfiguration configuration, DataContext dataContext, ILogger<AuthenticationController> logger)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            _logger.LogInformation("Starting the registration process.");

            User user = new User ();

            request.Password = "RegistroNuevo123**";

            if (request.Email == null || request.Email == "") 
            {
                return BadRequest("Email is required to signup.");
            }
            if (request.Password == null || request.Password == "")
            {
                return BadRequest("Password is required to signup.");
            }

            user.FullName = "";
            user.Username = "";
            user.Email = request.Email;
            user.PhoneNumber = "";
            user.BirthDate = null;
            user.Gender = "";
            user.City = "";
            user.IsEnabled = true;
            user.RegisterDate = DateTime.Now.ToLocalTime();

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            try
            {
                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();
                _logger.LogInformation("User registered successfully.");
                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has ocurred.");
                return BadRequest("Could not register user.");
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {
            var user = _dataContext.Users.Where(x => x.Email == request.Email).FirstOrDefault();

            if (user == null) 
            {
                return BadRequest("User not found");
            }
            if (!user.IsEnabled) 
            {
                return BadRequest("User blocked");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt)) 
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512(passwordSalt)) 
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
