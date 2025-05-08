using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Repositories;

namespace MinhaCarteira.API.Services
{
  public class AuthService : IAuthService
  {
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
      _userRepository = userRepository;
      _passwordHasher = passwordHasher;
      _configuration = configuration;
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
    {
      if (await _userRepository.AnyAsync(u => u.Email == registerDto.Email))
      {
        throw new InvalidOperationException("Email already exists");
      }

      var user = new User(
        registerDto.Name,
        registerDto.Email,
        _passwordHasher.HashPassword(registerDto.Password)
      );

      await _userRepository.AddAsync(user);
      await _userRepository.SaveChangesAsync();

      var token = GenerateJwtToken(user);

      return new AuthResponseDTO
      {
        Token = token,
        User = new UserDTO
        {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email,
          CreatedAt = user.CreatedAt,
          UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
        }
      };
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
    {
      var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

      if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
      {
        throw new InvalidOperationException("Invalid email or password");
      }

      var token = GenerateJwtToken(user);

      return new AuthResponseDTO
      {
        Token = token,
        User = new UserDTO
        {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email,
          CreatedAt = user.CreatedAt,
          UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
        }
      };
    }

    public async Task<UserDTO?> GetUserByIdAsync(int userId)
    {
      var user = await _userRepository.GetByIdAsync(userId);
      if (user == null)
      {
        return null;
      }

      return new UserDTO
      {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
      };
    }

    private string GenerateJwtToken(User user)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
        new Claim("id", user.Id.ToString()),
        new Claim("email", user.Email),
        new Claim("name", user.Name)
      };

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddDays(7),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}