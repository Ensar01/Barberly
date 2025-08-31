using Barberly.Interfaces;
using Barberly.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Barberly.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        public AuthController(ITokenService tokenService, IAuthService authService)
        {
            _tokenService = tokenService;
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("User registered successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
                return Unauthorized("Invalid email or password");
            return Ok(response);
        }

    }
}
