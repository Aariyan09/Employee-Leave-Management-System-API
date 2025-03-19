using Employee_Leave_System_Backend.Data;
using Employee_Leave_System_Backend.Entities.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee_Leave_System_Backend.Service
{
    public interface IAuthService
    {
        /// <summary>
        /// Logs in a user and returns a JWT token if authentication is successful.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <returns>JWT token string if successful, otherwise null.</returns>
        Task<string?> Login(string email, string password);

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="name">User's full name.</param>
        /// <param name="email">User's email (unique).</param>
        /// <param name="password">User's password.</param>
        /// <returns>True if registration is successful, otherwise false.</returns>
        Task<bool> Register(string name, string email, string password);

        /// <summary>
        /// Generates a JWT token for an authenticated user.
        /// </summary>
        /// <param name="user">User object.</param>
        /// <returns>JWT token as a string.</returns>
        string GenerateJwtToken(Users user);
    }


    public class AuthService : IAuthService
    {
        private readonly SQLDBContext _context;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of AuthService with dependencies.
        /// </summary>
        /// <param name="context">Database context for user management.</param>
        /// <param name="config">Configuration settings for JWT.</param>
        public AuthService(SQLDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        public async Task<string?> Login(string email, string password)
        {
            // Check if user exists in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null; // Invalid credentials

            // Generate JWT token for the authenticated user
            return GenerateJwtToken(user);
        }

        
        public async Task<bool> Register(string name, string email, string password)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return false;

            // Create a new user with a hashed password
            var user = new Users
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = Utility.Enums.RoleType.User // Default role as "User"
            };

            // Save user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateJwtToken(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Create JWT token
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _config["JwtSettings:Audience"],
                Issuer = _config["JwtSettings:Issuer"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var token = TokenHandler.CreateToken(TokenDescriptor);
            return TokenHandler.WriteToken(token);
        }
    }
}
