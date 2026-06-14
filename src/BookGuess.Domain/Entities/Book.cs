namespace BookGuess.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string? Genre { get; private set; }
    public string? Description { get; private set; }
    public int Difficulty { get; private set; }
    public int? PublishedYear { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<BookQuote> Quotes { get; private set; } = new List<BookQuote>();
    public ICollection<Trivia> Trivias { get; private set; } = new List<Trivia>();

    private Book() { }

    public static Book Create(string title, string author, int difficulty, string? genre = null, string? description = null, int? publishedYear = null)
    {
        if (difficulty < 1 || difficulty > 5)
            throw new ArgumentException("Difficulty must be between 1 and 5.");

        return new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            Genre = genre,
            Description = description,
            Difficulty = difficulty,
            PublishedYear = publishedYear,
            CreatedAt = DateTime.UtcNow
        };
    }
}
