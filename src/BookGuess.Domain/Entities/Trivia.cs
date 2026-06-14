namespace BookGuess.Domain.Entities;

public class Trivia
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public Book Book { get; private set; } = null!;

    private Trivia() { }

    public static Trivia Create(Guid bookId, string content, string category)
    {
        return new Trivia
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            Content = content,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };
    }
}
