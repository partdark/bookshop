using System.ComponentModel.DataAnnotations;

namespace Application.Dto
{
    public record RegisterDto(
        [Required, MaxLength(100)] string Name,
        [Required, EmailAddress, MaxLength(200)] string Email,
        [Required, MinLength(4), MaxLength(100)] string Password,
        [Phone, MaxLength(20)] string Phone,
        DateOnly DateOfBirth);

    public record LoginDto(
        [Required, EmailAddress] string Email,
        [Required] string Password);

    public record AuthResponseDto(string Token, string RefreshToken, CustomerResponseDto Customer, string Role);

    public record RefreshTokenDto([Required] string RefreshToken);

    public record ChangePasswordDto(
        [Required] string CurrentPassword,
        [Required, MinLength(4)] string NewPassword);
}
