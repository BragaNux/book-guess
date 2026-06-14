using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Auth;
using BookGuess.Application.Interfaces;
using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Auth;

public class RegisterUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IJwtService jwtService)
{
    public async Task<ApiResponse<AuthResponse>> ExecuteAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return ApiResponse<AuthResponse>.Fail("E-mail já cadastrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(user);
        return ApiResponse<AuthResponse>.Ok(new AuthResponse(token, user.Name, user.Email, user.Level, user.XP));
    }
}
