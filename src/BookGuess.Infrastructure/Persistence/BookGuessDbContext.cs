using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Persistence;

public class BookGuessDbContext(DbContextOptions<BookGuessDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookQuote> BookQuotes => Set<BookQuote>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Guess> Guesses => Set<Guess>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<DailyChallenge> DailyChallenges => Set<DailyChallenge>();
    public DbSet<Trivia> Trivias => Set<Trivia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookGuessDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
