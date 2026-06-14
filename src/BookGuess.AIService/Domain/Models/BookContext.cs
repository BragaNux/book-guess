namespace BookGuess.AIService.Domain.Models;

public record BookContext(
    Guid BookId,
    string Title,
    string Author,
    string? Description,
    IEnumerable<string> RelevantChunks
);
