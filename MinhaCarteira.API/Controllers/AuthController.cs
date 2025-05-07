using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

  [HttpPost("register")]
  public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDto)
  {
    try
    {
      var response = await _authService.RegisterAsync(registerDto);
      return Ok(response);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDto)
  {
    try
    {
      var response = await _authService.LoginAsync(loginDto);
      return Ok(response);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }
}