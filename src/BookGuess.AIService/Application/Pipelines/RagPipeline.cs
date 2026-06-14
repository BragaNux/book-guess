using BookGuess.AIService.Application.Prompts;
using BookGuess.AIService.Infrastructure.Claude;
using BookGuess.AIService.Infrastructure.Embeddings;
using BookGuess.AIService.Infrastructure.Qdrant;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BookGuess.AIService.Application.Pipelines;

public class RagPipeline(
    EmbeddingService embeddingService,
    QdrantService qdrantService,
    ClaudeService claudeService,
    IMemoryCache cache,
    ILogger<RagPipeline> logger)
{
    public async Task<string> GenerateHintAsync(Guid bookId, int hintLevel, string title, string author, string quote, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"hint_{bookId}_{hintLevel}";
        if (cache.TryGetValue(cacheKey, out string? cached))
            return cached!;

        var ragContext = await RetrieveContextAsync(bookId, $"dica nivel {hintLevel}", cancellationToken);
        var prompt = PromptTemplates.GenerateHint(hintLevel, title, author, quote, ragContext);
        var hint = await claudeService.CompleteAsync(prompt, cancellationToken);

        cache.Set(cacheKey, hint, TimeSpan.FromHours(6));

        logger.LogInformation("Generated hint for BookId={BookId}, Level={Level}", bookId, hintLevel);
        return hint;
    }

    public async Task<string> GenerateTriviaAsync(Guid bookId, string title, string author, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"trivia_{bookId}";
        if (cache.TryGetValue(cacheKey, out string? cached))
            return cached!;

        var ragContext = await RetrieveContextAsync(bookId, "curiosidade histórica sobre o livro", cancellationToken);
        var prompt = PromptTemplates.GenerateTrivia(title, author, ragContext);
        var trivia = await claudeService.CompleteAsync(prompt, cancellationToken);

        cache.Set(cacheKey, trivia, TimeSpan.FromHours(24));

        logger.LogInformation("Generated trivia for BookId={BookId}", bookId);
        return trivia;
    }

    public async Task<string> ExplainMistakeAsync(Guid bookId, string incorrectGuess, CancellationToken cancellationToken = default)
    {
        var context = await RetrieveContextAsync(bookId, incorrectGuess, cancellationToken);
        var prompt = PromptTemplates.ExplainMistake(context, incorrectGuess);
        var explanation = await claudeService.CompleteAsync(prompt, cancellationToken);

        logger.LogInformation("Generated explanation for BookId={BookId}", bookId);
        return explanation;
    }

    public async Task IngestBookAsync(Guid bookId, string title, string author, IEnumerable<string> textChunks, CancellationToken cancellationToken = default)
    {
        await qdrantService.EnsureCollectionExistsAsync(cancellationToken);

        var chunksWithVectors = new List<(string Text, float[] Vector)>();
        foreach (var chunk in textChunks)
        {
            var vector = await embeddingService.GenerateAsync(chunk, cancellationToken);
            chunksWithVectors.Add((chunk, vector));
        }

        await qdrantService.UpsertBookChunksAsync(bookId, title, author, chunksWithVectors, cancellationToken);
        logger.LogInformation("Ingested {Count} chunks for BookId={BookId}", chunksWithVectors.Count, bookId);
    }

    private async Task<string> RetrieveContextAsync(Guid bookId, string query, CancellationToken cancellationToken)
    {
        var queryVector = await embeddingService.GenerateAsync(query, cancellationToken);
        var chunks = await qdrantService.SearchRelevantChunksAsync(queryVector, bookId, 5, cancellationToken);
        return string.Join("\n\n", chunks);
    }
}
