using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;

        public AuthController(IAuthService authService, ICustomerService customerService)
        {
            _authService = authService;
            _customerService = customerService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.Register(dto);
            if (result == null)
                return BadRequest("Пользователь с таким email уже существует или данные некорректны.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<ActionResult<AuthResponseDto>> RegisterAdmin([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAdmin(dto);
            if (result == null)
                return BadRequest("Пользователь с таким email уже существует или данные некорректны.");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.Login(dto);
            if (result == null)
                return Unauthorized("Неверный email или пароль.");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CustomerResponseDto>> Me()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idStr == null || !Guid.TryParse(idStr, out var id))
                return Unauthorized();

            var customer = await _customerService.GetById(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idStr == null || !Guid.TryParse(idStr, out var id))
                return Unauthorized();

            var result = await _authService.ChangePassword(id, dto);
            if (!result) return BadRequest("Не удалось сменить пароль. Проверьте текущий пароль.");
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.Refresh(dto.RefreshToken);
            if (result == null) return Unauthorized("Недействительный или истёкший refresh токен.");
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idStr != null && Guid.TryParse(idStr, out var id))
                await _authService.RevokeToken(id);
            return Ok();
        }
    }
}
