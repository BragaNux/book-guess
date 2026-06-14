using BookGuess.AIService.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace BookGuess.AIService.Infrastructure.Qdrant;

public class QdrantService
{
    private readonly QdrantClient _client;
    private readonly ILogger<QdrantService> _logger;
    private const string CollectionName = "books";
    private const int VectorSize = 384;

    public QdrantService(IConfiguration configuration, ILogger<QdrantService> logger)
    {
        _logger = logger;
        var host = configuration["Qdrant:Host"] ?? "localhost";
        var port = int.Parse(configuration["Qdrant:Port"] ?? "6334");
        _client = new QdrantClient(host, port);
    }

    public async Task EnsureCollectionExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var collections = await _client.ListCollectionsAsync(cancellationToken);
            if (!collections.Any(c => c == CollectionName))
            {
                await _client.CreateCollectionAsync(CollectionName,
                    new VectorParams { Size = VectorSize, Distance = Distance.Cosine },
                    cancellationToken: cancellationToken);
                _logger.LogInformation("Created Qdrant collection '{Collection}'", CollectionName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure Qdrant collection exists");
        }
    }

    public async Task UpsertBookChunksAsync(Guid bookId, string title, string author, IEnumerable<(string Text, float[] Vector)> chunks, CancellationToken cancellationToken = default)
    {
        try
        {
            var points = chunks.Select((chunk, i) => new PointStruct
            {
                Id = new PointId { Uuid = Guid.NewGuid().ToString() },
                Vectors = chunk.Vector,
                Payload =
                {
                    ["bookId"] = bookId.ToString(),
                    ["title"] = title,
                    ["author"] = author,
                    ["text"] = chunk.Text,
                    ["chunkIndex"] = i
                }
            }).ToList();

            await _client.UpsertAsync(CollectionName, points, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert chunks for book {BookId}", bookId);
        }
    }

    public async Task<IEnumerable<string>> SearchRelevantChunksAsync(float[] queryVector, Guid bookId, int limit = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            var results = await _client.SearchAsync(
                CollectionName,
                queryVector,
                filter: new Filter
                {
                    Must = { new Condition { Field = new FieldCondition { Key = "bookId", Match = new Match { Keyword = bookId.ToString() } } } }
                },
                limit: (ulong)limit,
                cancellationToken: cancellationToken
            );

            return results.Select(r => r.Payload.TryGetValue("text", out var val) ? val.StringValue : string.Empty)
                          .Where(t => !string.IsNullOrEmpty(t));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search Qdrant for book {BookId}", bookId);
            return [];
        }
    }
}
