namespace BookGuess.Domain.Entities;

public class BookQuote
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public int Difficulty { get; private set; }
    public int? SourcePage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Book Book { get; private set; } = null!;

    private BookQuote() { }

    public static BookQuote Create(Guid bookId, string content, int difficulty, int? sourcePage = null)
    {
        if (difficulty < 1 || difficulty > 5)
            throw new ArgumentException("Difficulty must be between 1 and 5.");

        return new BookQuote
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            Content = content,
            Difficulty = difficulty,
            SourcePage = sourcePage,
            CreatedAt = DateTime.UtcNow
        };
    }
}
