using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Ranking;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Ranking;

public class GetRankingUseCase(IUserRepository userRepository)
{
    public async Task<ApiResponse<IEnumerable<RankingEntryDto>>> ExecuteAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetTopRankingAsync(limit, cancellationToken);

        var ranking = users
            .OrderByDescending(u => u.XP)
            .Select((u, index) => new RankingEntryDto(
                index + 1,
                u.Id,
                u.Name,
                u.AvatarUrl,
                u.XP,
                u.Level
            ));

        return ApiResponse<IEnumerable<RankingEntryDto>>.Ok(ranking);
    }
}
