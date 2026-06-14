using BookGuess.Application.DTOs;
using BookGuess.Application.DTOs.Auth;
using BookGuess.Application.Interfaces;
using BookGuess.Domain.Interfaces;

namespace BookGuess.Application.UseCases.Auth;

public class LoginUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IJwtService jwtService)
{
    public async Task<ApiResponse<AuthResponse>> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResponse>.Fail("E-mail ou senha inválidos.");

        user.UpdateLastLogin();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(user);
        return ApiResponse<AuthResponse>.Ok(new AuthResponse(token, user.Name, user.Email, user.Level, user.XP));
    }
}
