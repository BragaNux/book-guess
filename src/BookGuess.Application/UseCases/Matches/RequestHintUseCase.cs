using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Matches;
using BookGuess.Application.Interfaces;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Matches;

public class RequestHintUseCase(
    IMatchRepository matchRepository,
    IAIService aiService,
    IUnitOfWork unitOfWork)
{
    public async Task<ApiResponse<HintResponse>> ExecuteAsync(Guid matchId, Guid userId, CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match is null || match.UserId != userId)
            return ApiResponse<HintResponse>.Fail("Partida não encontrada.");

        if (!match.IsActive)
            return ApiResponse<HintResponse>.Fail("Esta partida já foi encerrada.");

        if (!match.UseHint())
            return ApiResponse<HintResponse>.Fail("Limite de dicas atingido.");

        var book = match.BookQuote.Book;
        var hint = await aiService.GenerateHintAsync(
            match.BookQuote.BookId, match.HintsUsed,
            book.Title, book.Author, match.BookQuote.Content,
            cancellationToken);

        await matchRepository.UpdateAsync(match, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<HintResponse>.Ok(new HintResponse(
            hint,
            Domain.Entities.Match.MaxHints - match.HintsUsed,
            match.Score
        ));
    }
}
