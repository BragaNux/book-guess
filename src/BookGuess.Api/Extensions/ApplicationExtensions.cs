using BookGuess.Application.UseCases.Auth;
using BookGuess.Application.UseCases.Matches;
using BookGuess.Application.UseCases.Profile;
using BookGuess.Application.UseCases.Ranking;

namespace BookGuess.Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationUseCases(this IServiceCollection services)
    {
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<StartMatchUseCase>();
        services.AddScoped<SubmitGuessUseCase>();
        services.AddScoped<RequestHintUseCase>();
        services.AddScoped<GetRankingUseCase>();
        services.AddScoped<GetProfileUseCase>();
        return services;
    }
}
