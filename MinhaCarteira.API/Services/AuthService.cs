using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Models;
using MinhaCarteira.API.Repositories;
using BCrypt.Net;

namespace MinhaCarteira.API.Services
{
  public class AuthService : IAuthService
  {
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
      _userRepository = userRepository;
      _jwtService = jwtService;
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
    {
      if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
      {
        throw new InvalidOperationException("Email já está em uso");
      }

      if (await _userRepository.GetByCPFAsync(registerDto.CPF) != null)
      {
        throw new InvalidOperationException("CPF já está em uso");
      }

      var user = new User(
        registerDto.Name,
        registerDto.Email,
        BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
        registerDto.CPF
      );

      await _userRepository.AddAsync(user);
      await _userRepository.SaveChangesAsync();

      var token = _jwtService.GenerateToken(user);

      return new AuthResponseDTO
      {
        Token = token,
        User = new UserDTO
        {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email,
          CPF = user.CPF
        }
      };
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
    {
      var user = await _userRepository.GetByEmailAsync(loginDto.Email);
      if (user == null)
      {
        throw new InvalidOperationException("Email ou senha inválidos");
      }

      if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
      {
        throw new InvalidOperationException("Email ou senha inválidos");
      }

      var token = _jwtService.GenerateToken(user);

      return new AuthResponseDTO
      {
        Token = token,
        User = new UserDTO
        {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email,
          CPF = user.CPF
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
        CPF = user.CPF,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
      };
    }
  }
}