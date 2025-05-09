using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.API.DTOs.Auth;
using MinhaCarteira.API.Services;
using MinhaCarteira.API.Models;
using BCrypt.Net;
using MinhaCarteira.API.DTOs.Reponses;

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
  [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterDTO registerDto)
  {
    try
    {
      var result = await _authService.RegisterAsync(registerDto);
      return Ok(ApiResponse<AuthResponseDTO>.CreateSuccess(result, "Usuário registrado com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<object>.CreateError(ex.Message));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [AllowAnonymous]
  [HttpPost("login")]
  [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login(LoginDTO loginDto)
  {
    try
    {
      var result = await _authService.LoginAsync(loginDto);
      return Ok(ApiResponse<AuthResponseDTO>.CreateSuccess(result, "Login realizado com sucesso"));
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ApiResponse<object>.CreateError(ex.Message));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }
  }

  [Authorize]
  [HttpGet("me")]
  [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<UserDTO>>> GetCurrentUser()
  {
    try
    {
      var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
      var user = await _authService.GetUserByIdAsync(userId);
      if (user == null)
      {
        return NotFound(ApiResponse<object>.CreateError("Usuário não encontrado"));
      }
      return Ok(ApiResponse<UserDTO>.CreateSuccess(user, "Usuário recuperado com sucesso"));
    }
    catch (Exception)
    {
      return StatusCode(500, ApiResponse<object>.CreateError("Ocorreu um erro interno ao processar sua solicitação"));
    }

  }
}