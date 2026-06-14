using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookGuess.AIService.Infrastructure.Embeddings;

public class EmbeddingService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<EmbeddingService> logger)
{
    private readonly string _model = configuration["Embeddings:Model"] ?? "all-minilm";
    private readonly string _provider = configuration["Embeddings:Provider"] ?? "ollama";

    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        if (_provider == "ollama")
            return await GenerateOllamaEmbeddingAsync(text, cancellationToken);

        return await GenerateClaudeCompatibleEmbeddingAsync(text, cancellationToken);
    }

    private async Task<float[]> GenerateOllamaEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient("Ollama");
            var payload = new { model = _model, prompt = text };
            var response = await client.PostAsJsonAsync("/api/embeddings", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken);
            return result?.Embedding ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate embedding via Ollama");
            return FallbackEmbedding(text);
        }
    }

    private Task<float[]> GenerateClaudeCompatibleEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        return Task.FromResult(FallbackEmbedding(text));
    }

    private static float[] FallbackEmbedding(string text)
    {
        var hash = text.GetHashCode();
        var rng = new Random(hash);
        return Enumerable.Range(0, 384).Select(_ => (float)rng.NextDouble()).ToArray();
    }

    private record OllamaEmbeddingResponse(float[] Embedding);
}
