namespace BookGuess.Application.DTOs.Ranking;

public record RankingEntryDto(int Position, Guid UserId, string Name, string? AvatarUrl, int Score, int Level);
