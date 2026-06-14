using BookGuess.Domain.Entities;

namespace BookGuess.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
