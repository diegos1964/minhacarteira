using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.DTOs;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Services;

namespace MinhaCarteira.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register(RegisterDTO registerDto)
  {
    try
    {
      var result = await _authService.RegisterAsync(registerDto);
      return Ok(ApiResponse<AuthResponseDTO>.CreateSuccess(result, "Usuário registrado com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<AuthResponseDTO>.CreateError(ex.Message));
    }
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login(LoginDTO loginDto)
  {
    try
    {
      var result = await _authService.LoginAsync(loginDto);
      return Ok(ApiResponse<AuthResponseDTO>.CreateSuccess(result, "Login realizado com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<AuthResponseDTO>.CreateError(ex.Message));
    }
  }

  [Authorize]
  [HttpGet("me")]
  public async Task<ActionResult<ApiResponse<UserDTO>>> GetCurrentUser()
  {
    var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
    var user = await _authService.GetUserByIdAsync(userId);
    if (user == null)
    {
      return NotFound(ApiResponse<UserDTO>.CreateError("Usuário não encontrado"));
    }
    return Ok(ApiResponse<UserDTO>.CreateSuccess(user, "Usuário recuperado com sucesso"));
  }
}