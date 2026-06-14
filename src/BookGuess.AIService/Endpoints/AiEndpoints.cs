using BookGuess.AIService.Application.Pipelines;
using BookGuess.AIService.Domain.Models;

namespace BookGuess.AIService.Endpoints;

public static class AiEndpoints
{
    public static void MapAiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ai");

        group.MapPost("/hint", async (HintRequest request, RagPipeline pipeline, CancellationToken ct) =>
        {
            var hint = await pipeline.GenerateHintAsync(request.BookId, request.HintLevel, request.Title, request.Author, request.Quote, ct);
            return Results.Ok(new { Hint = hint });
        });

        group.MapPost("/trivia", async (TriviaRequest request, RagPipeline pipeline, CancellationToken ct) =>
        {
            var trivia = await pipeline.GenerateTriviaAsync(request.BookId, request.Title, request.Author, ct);
            return Results.Ok(new { Trivia = trivia });
        });

        group.MapPost("/explain", async (ExplainRequest request, RagPipeline pipeline, CancellationToken ct) =>
        {
            var explanation = await pipeline.ExplainMistakeAsync(request.BookId, request.IncorrectGuess, ct);
            return Results.Ok(new { Explanation = explanation });
        });

        group.MapPost("/ingest", async (IngestRequest request, RagPipeline pipeline, CancellationToken ct) =>
        {
            await pipeline.IngestBookAsync(request.BookId, request.Title, request.Author, request.Chunks, ct);
            return Results.Ok(new { Success = true });
        });

        group.MapGet("/health", () => Results.Ok(new { Status = "healthy" }));
    }
}
