using BookGuess.AIService.Application.Pipelines;
using BookGuess.AIService.Endpoints;
using BookGuess.AIService.Infrastructure.Claude;
using BookGuess.AIService.Infrastructure.Embeddings;
using BookGuess.AIService.Infrastructure.Qdrant;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<QdrantService>();
builder.Services.AddScoped<EmbeddingService>();
builder.Services.AddScoped<ClaudeService>();
builder.Services.AddScoped<RagPipeline>();

builder.Services.AddHttpClient("Ollama", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Embeddings:OllamaUrl"] ?? "http://localhost:11434");
});

var app = builder.Build();

app.MapAiEndpoints();

app.Run();
