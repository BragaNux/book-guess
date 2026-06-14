using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Matches;
using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Matches;

public class StartMatchUseCase(
    IMatchRepository matchRepository,
    IBookQuoteRepository quoteRepository,
    IDailyChallengeRepository dailyChallengeRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ApiResponse<StartMatchResponse>> ExecuteAsync(Guid userId, bool daily = false, CancellationToken cancellationToken = default)
    {
        // Se já existe partida ativa, retorna o estado atual em vez de falhar
        var existing = await matchRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            return ApiResponse<StartMatchResponse>.Ok(new StartMatchResponse(
                existing.Id,
                existing.BookQuote.Content,
                existing.BookQuote.Difficulty,
                Match.MaxAttempts - existing.Attempts,
                Match.MaxHints - existing.HintsUsed,
                existing.Score,
                existing.IsDaily
            ));
        }

        BookQuote? quote;

        if (daily)
        {
            var challenge = await dailyChallengeRepository.GetByDateAsync(DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
            if (challenge is null)
            {
                // Fallback: partida livre se não houver desafio diário
                quote = await quoteRepository.GetRandomAsync(cancellationToken);
                if (quote is null)
                    return ApiResponse<StartMatchResponse>.Fail("Nenhum trecho disponível no momento.");
                daily = false;
            }
            else
            {
                quote = challenge.BookQuote;
            }
        }
        else
        {
            quote = await quoteRepository.GetRandomAsync(cancellationToken);
            if (quote is null)
                return ApiResponse<StartMatchResponse>.Fail("Nenhum trecho disponível no momento.");
        }

        var match = Match.Create(userId, quote.Id, daily);
        await matchRepository.AddAsync(match, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<StartMatchResponse>.Ok(new StartMatchResponse(
            match.Id,
            quote.Content,
            quote.Difficulty,
            Match.MaxAttempts - match.Attempts,
            Match.MaxHints - match.HintsUsed,
            match.Score,
            match.IsDaily
        ));
    }
}
