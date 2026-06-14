using BookGuess.Domain.Enums;

namespace BookGuess.Domain.Entities;

public class Guess
{
    public Guid Id { get; private set; }
    public Guid MatchId { get; private set; }
    public string GuessText { get; private set; } = string.Empty;
    public GuessType GuessType { get; private set; }
    public bool IsCorrect { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Match Match { get; private set; } = null!;

    private Guess() { }

    public static Guess Create(Guid matchId, string guessText, GuessType guessType, bool isCorrect)
    {
        return new Guess
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            GuessText = guessText,
            GuessType = guessType,
            IsCorrect = isCorrect,
            CreatedAt = DateTime.UtcNow
        };
    }
}
