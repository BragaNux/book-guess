using System.Net.Http.Json;
using System.Text.Json;
using BookGuess.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookGuess.Infrastructure.Services;

public class AIServiceClient(IHttpClientFactory httpClientFactory, ILogger<AIServiceClient> logger) : IAIService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("AIService");

    public async Task<string> GenerateHintAsync(Guid bookId, int hintLevel, string title, string author, string quote, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/ai/hint", new { bookId, hintLevel, title, author, quote }, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<HintResult>(cancellationToken: cancellationToken);
            return json?.Hint ?? "Dica não disponível.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling AI Service for hint. BookId={BookId}", bookId);
            return "Dica temporariamente indisponível.";
        }
    }

    public async Task<string> GenerateTriviaAsync(Guid bookId, string title, string author, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/ai/trivia", new { bookId, title, author }, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<TriviaResult>(cancellationToken: cancellationToken);
            return json?.Trivia ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling AI Service for trivia. BookId={BookId}", bookId);
            return string.Empty;
        }
    }

    public async Task<string> ExplainMistakeAsync(Guid bookId, string incorrectGuess, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/ai/explain", new { bookId, incorrectGuess }, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<ExplainResult>(cancellationToken: cancellationToken);
            return json?.Explanation ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling AI Service for explanation. BookId={BookId}", bookId);
            return string.Empty;
        }
    }

    private record HintResult(string Hint);
    private record TriviaResult(string Trivia);
    private record ExplainResult(string Explanation);
}
