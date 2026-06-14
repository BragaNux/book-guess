using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookGuess.Infrastructure.Persistence;

public class BookGuessDbContextFactory : IDesignTimeDbContextFactory<BookGuessDbContext>
{
    public BookGuessDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookGuessDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=bookguess;User=bookguess;Password=bookguess123;",
            new MySqlServerVersion(new Version(8, 0, 0))
        );
        return new BookGuessDbContext(optionsBuilder.Options);
    }
}
