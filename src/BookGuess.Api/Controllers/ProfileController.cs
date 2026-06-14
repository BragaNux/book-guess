using System.Security.Claims;
using BookGuess.Application.UseCases.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookGuess.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController(GetProfileUseCase getProfileUseCase) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await getProfileUseCase.ExecuteAsync(CurrentUserId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
