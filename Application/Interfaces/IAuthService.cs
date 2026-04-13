using Application.Dto;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> Register(RegisterDto dto);
        Task<AuthResponseDto?> RegisterAdmin(RegisterDto dto);
        Task<AuthResponseDto?> Login(LoginDto dto);
        Task<AuthResponseDto?> Refresh(string refreshToken);
        Task<bool> ChangePassword(Guid customerId, ChangePasswordDto dto);
        Task RevokeToken(Guid customerId);
    }
}
