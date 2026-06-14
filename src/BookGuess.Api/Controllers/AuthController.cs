using BookGuess.Application.DTOs.Auth;
using BookGuess.Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BookGuess.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RegisterUseCase registerUseCase, LoginUseCase loginUseCase) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await registerUseCase.ExecuteAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await loginUseCase.ExecuteAsync(request, cancellationToken);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
