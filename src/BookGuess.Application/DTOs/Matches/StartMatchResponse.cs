namespace BookGuess.Application.DTOs.Matches;

public record StartMatchResponse(
    Guid MatchId,
    string Quote,
    int Difficulty,
    int AttemptsRemaining,
    int HintsRemaining,
    int Score,
    bool IsDaily
);
