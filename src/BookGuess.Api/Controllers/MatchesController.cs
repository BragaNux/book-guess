using System.Security.Claims;
using BookGuess.Application.DTOs.Matches;
using BookGuess.Application.UseCases.Matches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookGuess.Api.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController(
    StartMatchUseCase startMatchUseCase,
    SubmitGuessUseCase submitGuessUseCase,
    RequestHintUseCase requestHintUseCase) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromQuery] bool daily = false, CancellationToken cancellationToken = default)
    {
        var result = await startMatchUseCase.ExecuteAsync(CurrentUserId, daily, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/guess")]
    public async Task<IActionResult> Guess(Guid id, [FromBody] SubmitGuessRequest request, CancellationToken cancellationToken)
    {
        var result = await submitGuessUseCase.ExecuteAsync(id, CurrentUserId, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/hint")]
    public async Task<IActionResult> Hint(Guid id, CancellationToken cancellationToken)
    {
        var result = await requestHintUseCase.ExecuteAsync(id, CurrentUserId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
