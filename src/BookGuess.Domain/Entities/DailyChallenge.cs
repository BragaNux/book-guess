namespace BookGuess.Domain.Entities;

public class DailyChallenge
{
    public Guid Id { get; private set; }
    public Guid BookQuoteId { get; private set; }
    public DateOnly ChallengeDate { get; private set; }
    public int Difficulty { get; private set; }

    public BookQuote BookQuote { get; private set; } = null!;

    private DailyChallenge() { }

    public static DailyChallenge Create(Guid bookQuoteId, DateOnly challengeDate, int difficulty)
    {
        return new DailyChallenge
        {
            Id = Guid.NewGuid(),
            BookQuoteId = bookQuoteId,
            ChallengeDate = challengeDate,
            Difficulty = difficulty
        };
    }
}
