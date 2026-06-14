using BookGuess.Application.Interfaces;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Identity;
using BookGuess.Infrastructure.Persistence;
using BookGuess.Infrastructure.Repositories;
using BookGuess.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookGuess.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<BookGuessDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BookGuessDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookQuoteRepository, BookQuoteRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IDailyChallengeRepository, DailyChallengeRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<DataSeeder>();

        return services;
    }
}
