using Azure.Core;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            var isRegistred = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (isRegistred != null) 
            {
                return BadRequest("User already registered.");
            }

            try
            {

                var userRecordArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.Email
                };

                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

                var user = new User
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    Username = request.UserName,
                    //falta hasear la contrasena
                    RegisterDate = DateTime.Now,
                    IsEnabled = true,
                    FirebaseUid = userRecord.Uid
                };

                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has ocurred.");
                return BadRequest("Could not register user." + e);
                throw;
            }
        }

        [HttpPost("register-social")]
        public async Task<ActionResult<string>> Register(string idToken)
        {
            try
            {
                var verifiedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                var uid = verifiedToken.Uid;
                var email = verifiedToken.Claims["email"]?.ToString();

                var isRegistred = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (isRegistred != null)
                {
                    return BadRequest("User already registered.");
                }

                var user = new User
                {
                    Email = email,
                    RegisterDate = DateTime.Now,
                    IsEnabled = true,
                    FirebaseUid = uid
                };

                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "User registered successfully", Uid = uid });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error registering user: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> SocialLogin(string idToken)
        {
            try
            {
                var verifiedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                var uid = verifiedToken.Uid;

                return Ok(new { Message = "Login successful", Uid = uid });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid token: {ex.Message}");
            }
        }

        /* [HttpPost("login")]
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


             string token = CreateToken(user);

             return Ok(token);
         }
        */
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
