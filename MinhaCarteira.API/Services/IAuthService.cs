using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Models;

namespace MinhaCarteira.API.Services;

public interface IAuthService
{
  Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
  Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
  Task<UserDTO?> GetUserByIdAsync(int userId);
}