using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityMS.Api.DTOs;
using UniversityMS.Application.Features.Authentication.Commands;
using UniversityMS.Application.Features.Authentication.DTOs;

namespace UniversityMS.Api.Controllers.v1;

public class AuthController : BaseApiController
{
    /// <summary>
    /// Kullanıcı girişi
    /// </summary>
    /// <param name="loginDto">Login bilgileri</param>
    /// <returns>JWT Token ve kullanıcı bilgileri</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var command = new LoginCommand(loginDto.Username, loginDto.Password);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Message, errors = result.Errors });

        return Ok(result);
    }

    /// <summary>
    /// Refresh token ile yeni access token alma
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token bilgileri</param>
    /// <returns>Yeni JWT Token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
    {
        var command = new RefreshTokenCommand(
            refreshTokenDto.AccessToken,
            refreshTokenDto.RefreshToken);

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Message, errors = result.Errors });

        return Ok(result);
    }

    /// <summary>
    /// Kullanıcı çıkışı
    /// </summary>
    /// <returns>Başarı mesajı</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz kullanıcı." });
        }

        var command = new LogoutCommand(userId);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Mevcut kullanıcı bilgilerini getir
    /// </summary>
    /// <returns>Kullanıcı bilgileri</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            userId,
            username,
            email,
            roles,
            isAuthenticated = true
        });
    }

    /// <summary>
    /// Token doğrulama
    /// </summary>
    /// <returns>Token geçerlilik durumu</returns>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        return Ok(new { valid = true, message = "Token geçerli." });
    }
}