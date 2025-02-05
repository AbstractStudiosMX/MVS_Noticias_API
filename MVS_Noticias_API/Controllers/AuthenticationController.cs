using Azure.Core;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DataService;
using MVS_Noticias_API.Models.Domain;
using MVS_Noticias_API.Models.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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


                if (isRegistred.Username == request.UserName)
                {
                    return BadRequest("Username already taken.");
                }

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
                    FirebaseUid = userRecord.Uid,
                    Provider = "password"
                };

                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();

                var userNew = await _dataContext.Users.Where(x => x.FirebaseUid == userRecord.Uid).FirstOrDefaultAsync();

                TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoCityTimeZone);

                var notificationSettings = new NotificationsSettings
                {
                    UserId = userNew.Id,
                    Tendencias = true,
                    Entrevistas = true,
                    MVSDeportes = true,
                    Nacional = true,
                    Videos = true,
                    CDMX = true,
                    Entretenimiento = true,
                    Opinion = true,
                    Economia = true,
                    Estados = true,
                    Mundo = true,
                    Mascotas = true,
                    SaludBienestar = true,
                    Policiaca = true,
                    Programacion = true,
                    CienciaTecnologia = true,
                    Viral = true,
                    StartTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0),
                    EndTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0),
                };

                await _dataContext.NotificationsSettings.AddAsync(notificationSettings);
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
        public async Task<ActionResult<string>> Register(string idToken, string userEmail, string provider)
        {
            try
            {
                var verifiedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                var uid = verifiedToken.Uid;
                var email = userEmail;

                var isRegistered = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Provider == provider);

                if (isRegistered != null)
                {
                    return BadRequest("User already registered.");
                }

                string displayName = verifiedToken.Claims.ContainsKey("name") ? verifiedToken.Claims["name"].ToString() : "User";

                string sanitizedUsername = RemoveSpecialCharacters(displayName).Replace(" ", "");
                string username = await GenerateUniqueUsername(sanitizedUsername);

                var user = new User
                {
                    Email = email,
                    RegisterDate = DateTime.Now,
                    IsEnabled = true,
                    FirebaseUid = uid,
                    Provider = provider,
                    Username = username
                };

                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();

                var userNew = await _dataContext.Users.Where(x => x.FirebaseUid == uid).FirstOrDefaultAsync();

                TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoCityTimeZone);

                var notificationSettings = new NotificationsSettings
                {
                    UserId = userNew.Id,
                    Tendencias = true,
                    Entrevistas = true,
                    MVSDeportes = true,
                    Nacional = true,
                    Videos = true,
                    CDMX = true,
                    Entretenimiento = true,
                    Opinion = true,
                    Economia = true,
                    Estados = true,
                    Mundo = true,
                    Mascotas = true,
                    SaludBienestar = true,
                    Policiaca = true,
                    Programacion = true,
                    CienciaTecnologia = true,
                    Viral = true,
                    StartTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0),
                    EndTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0),
                };

                await _dataContext.NotificationsSettings.AddAsync(notificationSettings);
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "User registered successfully", Uid = uid, Username = username });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error registering user: {ex.Message}");
            }
        }

        private async Task<string> GenerateUniqueUsername(string baseUsername)
        {
            string username = baseUsername;
            Random random = new Random();
           
            username = baseUsername + random.Next(0, 999);

            return username;
        }

        private string RemoveSpecialCharacters(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9]", ""); // Solo deja letras y números
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> SocialLogin(string idToken, string userEmail, string provider)
        {
            try
            {
                var verifiedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var uid = verifiedToken.Uid;
                var email = userEmail;

                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Provider == provider);

                if (user != null)
                {
                    return Ok(new { Message = "Login successful", Uid = user.FirebaseUid, Username = user.Username });
                }

                // Si el usuario no está registrado, proceder con el registro automático
                string displayName = verifiedToken.Claims.ContainsKey("name") ? verifiedToken.Claims["name"].ToString() : "User";
                string sanitizedUsername = RemoveSpecialCharacters(displayName).Replace(" ", "");
                string username = await GenerateUniqueUsername(sanitizedUsername);

                var newUser = new User
                {
                    Email = email,
                    RegisterDate = DateTime.Now,
                    IsEnabled = true,
                    FirebaseUid = uid,
                    Provider = provider,
                    Username = username
                };

                _dataContext.Users.Add(newUser);
                await _dataContext.SaveChangesAsync();

                var userNew = await _dataContext.Users.FirstOrDefaultAsync(x => x.FirebaseUid == uid);

                TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoCityTimeZone);

                var notificationSettings = new NotificationsSettings
                {
                    UserId = userNew.Id,
                    Tendencias = true,
                    Entrevistas = true,
                    MVSDeportes = true,
                    Nacional = true,
                    Videos = true,
                    CDMX = true,
                    Entretenimiento = true,
                    Opinion = true,
                    Economia = true,
                    Estados = true,
                    Mundo = true,
                    Mascotas = true,
                    SaludBienestar = true,
                    Policiaca = true,
                    Programacion = true,
                    CienciaTecnologia = true,
                    Viral = true,
                    StartTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0),
                    EndTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0),
                };

                await _dataContext.NotificationsSettings.AddAsync(notificationSettings);
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "User registered and logged in successfully", Uid = uid, Username = username });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid token: {ex.Message}");
            }
        }

        [HttpGet("emailFromUser")]
        public async Task<ActionResult<List<object>>> GetEmailFromUser(string userName)
        {
            _logger.LogInformation("Starting getting email from user.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Username == userName);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting all saved news: " + ex.Message);
                return BadRequest("Error getting all saved news: " + ex.Message);
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
