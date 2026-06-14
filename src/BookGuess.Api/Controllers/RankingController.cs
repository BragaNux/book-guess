using BookGuess.Application.UseCases.Ranking;
using Microsoft.AspNetCore.Mvc;

namespace BookGuess.Api.Controllers;

[ApiController]
[Route("api/ranking")]
public class RankingController(GetRankingUseCase getRankingUseCase) : ControllerBase
{
    [HttpGet("global")]
    public async Task<IActionResult> Global(CancellationToken cancellationToken)
    {
        var result = await getRankingUseCase.ExecuteAsync(50, cancellationToken);
        return Ok(result);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> Weekly(CancellationToken cancellationToken)
    {
        var result = await getRankingUseCase.ExecuteAsync(50, cancellationToken);
        return Ok(result);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly(CancellationToken cancellationToken)
    {
        var result = await getRankingUseCase.ExecuteAsync(50, cancellationToken);
        return Ok(result);
    }
}
