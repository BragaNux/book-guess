using BookGuess.Domain.Enums;

namespace BookGuess.Domain.Entities;

public class Match
{
    public const int MaxAttempts = 5;
    public const int MaxHints = 3;
    public const int BaseScore = 1000;
    public const int ScorePenaltyPerError = 100;
    public const int ScorePenaltyPerHint = 150;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid BookQuoteId { get; private set; }
    public MatchStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public int HintsUsed { get; private set; }
    public int Score { get; private set; }
    public bool IsDaily { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }

    public User User { get; private set; } = null!;
    public BookQuote BookQuote { get; private set; } = null!;
    public ICollection<Guess> Guesses { get; private set; } = new List<Guess>();

    private Match() { }

    public static Match Create(Guid userId, Guid bookQuoteId, bool isDaily = false)
    {
        return new Match
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookQuoteId = bookQuoteId,
            Status = MatchStatus.Active,
            Attempts = 0,
            HintsUsed = 0,
            Score = BaseScore,
            IsDaily = isDaily,
            StartedAt = DateTime.UtcNow
        };
    }

    public void RegisterIncorrectGuess()
    {
        Attempts++;
        Score = Math.Max(0, Score - ScorePenaltyPerError);

        if (Attempts >= MaxAttempts)
            Lose();
    }

    public void RegisterCorrectGuess()
    {
        Attempts++;
        Status = MatchStatus.Won;
        FinishedAt = DateTime.UtcNow;
    }

    public bool UseHint()
    {
        if (HintsUsed >= MaxHints) return false;
        HintsUsed++;
        Score = Math.Max(0, Score - ScorePenaltyPerHint);
        return true;
    }

    public void Abandon()
    {
        Status = MatchStatus.Abandoned;
        FinishedAt = DateTime.UtcNow;
    }

    public int CalculateXP()
    {
        if (Status != MatchStatus.Won) return 0;

        int xp = 100;

        if (Attempts == 1)
            xp += 50;

        if (HintsUsed == 0)
            xp += 25;

        if (IsDaily)
            xp += 100;

        return xp;
    }

    public bool IsActive => Status == MatchStatus.Active;

    private void Lose()
    {
        Status = MatchStatus.Lost;
        FinishedAt = DateTime.UtcNow;
    }
}
