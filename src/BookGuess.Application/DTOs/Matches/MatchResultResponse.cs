namespace BookGuess.Application.DTOs.Matches;

public record MatchResultResponse(
    string BookTitle,
    string Author,
    int Score,
    int XPEarned,
    string? Trivia
);
