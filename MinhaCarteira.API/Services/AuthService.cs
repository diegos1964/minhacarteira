using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Repositories;
using BCrypt.Net;

namespace MinhaCarteira.API.Services;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;
  private readonly JwtService _jwtService;

  public AuthService(IUserRepository userRepository, JwtService jwtService)
  {
    _userRepository = userRepository;
    _jwtService = jwtService;
  }

  public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
  {
    if (await _userRepository.AnyAsync(u => u.Email == registerDto.Email))
    {
      throw new InvalidOperationException("Email já está em uso");
    }

    var user = new User
    {
      Name = registerDto.Name,
      Email = registerDto.Email,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
    };

    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();

    var token = _jwtService.GenerateToken(user);

    return new AuthResponseDTO
    {
      Token = token,
      Name = user.Name,
      Email = user.Email
    };
  }

  public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
  {
    var user = await _userRepository.GetByEmailAsync(loginDto.Email);

    if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
    {
      throw new InvalidOperationException("Email ou senha inválidos");
    }

    var token = _jwtService.GenerateToken(user);

    return new AuthResponseDTO
    {
      Token = token,
      Name = user.Name,
      Email = user.Email
    };
  }
}