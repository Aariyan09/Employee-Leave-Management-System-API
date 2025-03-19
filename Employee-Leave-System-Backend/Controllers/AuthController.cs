using Employee_Leave_System_Backend.Entities.DTO;
using Employee_Leave_System_Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Leave_System_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        #region Initialization
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion


        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">User login credentials.</param>
        /// <returns>JWT token if successful, otherwise Unauthorized.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.Login(loginDto.Email, loginDto.Password);
            if (token is null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDto">User registration details.</param>
        /// <returns>Success or failure response.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var success = await _authService.Register(registerDto.Name, registerDto.Email, registerDto.Password);
            if (!success)
                return BadRequest(new { message = "Email already exists" });

            return Ok(new { message = "User registered successfully" });
        }
    }
}
