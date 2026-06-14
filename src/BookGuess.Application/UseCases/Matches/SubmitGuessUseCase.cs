using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Matches;
using BookGuess.Application.Interfaces;
using BookGuess.Domain.Entities;
using BookGuess.Domain.Enums;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Matches;

public class SubmitGuessUseCase(
    IMatchRepository matchRepository,
    IUserRepository userRepository,
    IAchievementRepository achievementRepository,
    IAIService aiService,
    IUnitOfWork unitOfWork)
{
    public async Task<ApiResponse<GuessResponse>> ExecuteAsync(Guid matchId, Guid userId, SubmitGuessRequest request, CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match is null || match.UserId != userId)
            return ApiResponse<GuessResponse>.Fail("Partida não encontrada.");

        if (!match.IsActive)
            return ApiResponse<GuessResponse>.Fail("Esta partida já foi encerrada.");

        if (!Enum.TryParse<GuessType>(request.GuessType, true, out var guessType))
            return ApiResponse<GuessResponse>.Fail("Tipo de resposta inválido. Use 'Book' ou 'Author'.");

        var book = match.BookQuote.Book;
        var isCorrect = guessType == GuessType.Book
            ? NormalizeText(request.GuessText) == NormalizeText(book.Title)
            : NormalizeText(request.GuessText) == NormalizeText(book.Author);

        var guess = Guess.Create(matchId, request.GuessText, guessType, isCorrect);

        MatchResultResponse? result = null;

        if (isCorrect)
        {
            match.RegisterCorrectGuess();

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
            {
                var xp = match.CalculateXP();
                user.AddXP(xp);
                user.IncrementStreak();
                await CheckAndAwardAchievementsAsync(user, match, achievementRepository, unitOfWork, cancellationToken);

                string? trivia = null;
                try { trivia = await aiService.GenerateTriviaAsync(book.Id, book.Title, book.Author, cancellationToken); }
                catch { /* não bloquear resultado por falha na IA */ }

                result = new MatchResultResponse(book.Title, book.Author, match.Score, xp, trivia);
            }
        }
        else
        {
            match.RegisterIncorrectGuess();

            if (match.Status == Domain.Enums.MatchStatus.Lost)
            {
                string? trivia = null;
                try { trivia = await aiService.GenerateTriviaAsync(book.Id, book.Title, book.Author, cancellationToken); }
                catch { }
                result = new MatchResultResponse(book.Title, book.Author, match.Score, 0, trivia);
            }
        }

        await matchRepository.UpdateAsync(match, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<GuessResponse>.Ok(new GuessResponse(
            isCorrect,
            Match.MaxAttempts - match.Attempts,
            match.Score,
            match.Status.ToString(),
            result
        ));
    }

    private static string NormalizeText(string text) =>
        text.Trim().ToLowerInvariant()
            .Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a")
            .Replace("é", "e").Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o").Replace("õ", "o").Replace("ô", "o")
            .Replace("ú", "u")
            .Replace("ç", "c");

    private static async Task CheckAndAwardAchievementsAsync(
        User user, Match match,
        IAchievementRepository repo,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var winsCount = await repo.GetByUserIdAsync(user.Id, cancellationToken);
        var codes = new List<string>();

        if (!await repo.UserHasAchievementAsync(user.Id, "FIRST_WIN", cancellationToken))
            codes.Add("FIRST_WIN");

        var totalWins = winsCount.Count();
        if (totalWins >= 10 && !await repo.UserHasAchievementAsync(user.Id, "TEN_BOOKS", cancellationToken))
            codes.Add("TEN_BOOKS");
        if (totalWins >= 50 && !await repo.UserHasAchievementAsync(user.Id, "FIFTY_BOOKS", cancellationToken))
            codes.Add("FIFTY_BOOKS");
        if (totalWins >= 100 && !await repo.UserHasAchievementAsync(user.Id, "HUNDRED_BOOKS", cancellationToken))
            codes.Add("HUNDRED_BOOKS");

        if (user.CurrentStreak >= 7 && !await repo.UserHasAchievementAsync(user.Id, "SEVEN_DAY_STREAK", cancellationToken))
            codes.Add("SEVEN_DAY_STREAK");

        foreach (var code in codes)
        {
            var achievement = await repo.GetByCodeAsync(code, cancellationToken);
            if (achievement is null) continue;
            var ua = UserAchievement.Create(user.Id, achievement.Id);
            await repo.AddUserAchievementAsync(ua, cancellationToken);
            user.AddXP(achievement.XPReward);
        }
    }
}
