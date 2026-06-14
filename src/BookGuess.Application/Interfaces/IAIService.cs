namespace BookGuess.Application.Interfaces;

public interface IAIService
{
    Task<string> GenerateHintAsync(Guid bookId, int hintLevel, string title, string author, string quote, CancellationToken cancellationToken = default);
    Task<string> GenerateTriviaAsync(Guid bookId, string title, string author, CancellationToken cancellationToken = default);
    Task<string> ExplainMistakeAsync(Guid bookId, string incorrectGuess, CancellationToken cancellationToken = default);
}
