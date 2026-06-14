using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Profile;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Profile;

public class GetProfileUseCase(IUserRepository userRepository, IAchievementRepository achievementRepository, IMatchRepository matchRepository)
{
    public async Task<ApiResponse<ProfileResponse>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return ApiResponse<ProfileResponse>.Fail("Usuário não encontrado.");

        var userAchievements = await achievementRepository.GetByUserIdAsync(userId, cancellationToken);
        var totalWins = await matchRepository.CountWonByUserIdAsync(userId, cancellationToken);

        var achievements = userAchievements.Select(ua => new AchievementDto(
            ua.Achievement.Code,
            ua.Achievement.Name,
            ua.Achievement.Description,
            ua.Achievement.Icon,
            ua.UnlockedAt
        ));

        return ApiResponse<ProfileResponse>.Ok(new ProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            user.AvatarUrl,
            user.Level,
            user.XP,
            user.CurrentStreak,
            user.BestStreak,
            totalWins,
            achievements
        ));
    }
}
