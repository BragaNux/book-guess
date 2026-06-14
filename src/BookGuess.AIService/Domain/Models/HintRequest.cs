namespace BookGuess.AIService.Domain.Models;

public record HintRequest(Guid BookId, int HintLevel, string Title, string Author, string Quote);
public record TriviaRequest(Guid BookId, string Title, string Author);
public record ExplainRequest(Guid BookId, string IncorrectGuess);
public record IngestRequest(Guid BookId, string Title, string Author, string? Description, IEnumerable<string> Chunks);
