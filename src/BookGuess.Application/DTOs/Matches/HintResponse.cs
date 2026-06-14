namespace BookGuess.Application.DTOs.Matches;

public record HintResponse(string Hint, int HintsRemaining, int Score);
