using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookGuess.AIService.Infrastructure.Claude;

public class ClaudeService
{
    private readonly AnthropicClient _client;
    private readonly ILogger<ClaudeService> _logger;
    private const string Model = "claude-haiku-4-5-20251001";

    public ClaudeService(IConfiguration configuration, ILogger<ClaudeService> logger)
    {
        _logger = logger;
        var apiKey = configuration["Claude:ApiKey"] ?? throw new InvalidOperationException("Claude API key not configured.");
        _client = new AnthropicClient(apiKey);
    }

    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new MessageParameters
            {
                Model = Model,
                MaxTokens = 300,
                Messages = [new Message(RoleType.User, prompt)]
            };

            var response = await _client.Messages.GetClaudeMessageAsync(request, cancellationToken);
            return response.Message.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Claude API");
            throw;
        }
    }
}
