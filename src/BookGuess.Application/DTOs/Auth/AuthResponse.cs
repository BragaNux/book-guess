namespace BookGuess.Application.DTOs.Auth;

public record AuthResponse(string Token, string Name, string Email, int Level, int XP);
