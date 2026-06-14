using BookGuess.Domain.Entities;
using FluentAssertions;

namespace BookGuess.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ShouldInitializeWithLevel1AndZeroXP()
    {
        var user = User.Create("João", "joao@test.com", "hash");

        user.Level.Should().Be(1);
        user.XP.Should().Be(0);
        user.CurrentStreak.Should().Be(0);
        user.BestStreak.Should().Be(0);
    }

    [Fact]
    public void AddXP_ShouldAccumulateXP()
    {
        var user = User.Create("João", "joao@test.com", "hash");

        user.AddXP(100);
        user.AddXP(50);

        user.XP.Should().Be(150);
    }

    [Fact]
    public void AddXP_ShouldUpdateLevelWhenThresholdReached()
    {
        var user = User.Create("João", "joao@test.com", "hash");

        user.AddXP(500);

        user.Level.Should().Be(2);
    }

    [Fact]
    public void AddXP_WithNegativeOrZeroAmount_ShouldNotChange()
    {
        var user = User.Create("João", "joao@test.com", "hash");

        user.AddXP(0);
        user.AddXP(-100);

        user.XP.Should().Be(0);
    }

    [Fact]
    public void IncrementStreak_ShouldUpdateBestStreak()
    {
        var user = User.Create("João", "joao@test.com", "hash");

        user.IncrementStreak();
        user.IncrementStreak();
        user.IncrementStreak();

        user.CurrentStreak.Should().Be(3);
        user.BestStreak.Should().Be(3);
    }

    [Fact]
    public void ResetStreak_ShouldKeepBestStreakUnchanged()
    {
        var user = User.Create("João", "joao@test.com", "hash");
        user.IncrementStreak();
        user.IncrementStreak();

        user.ResetStreak();

        user.CurrentStreak.Should().Be(0);
        user.BestStreak.Should().Be(2);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(499, 1)]
    [InlineData(500, 2)]
    [InlineData(1000, 3)]
    [InlineData(2000, 4)]
    [InlineData(3500, 5)]
    [InlineData(9999, 5)]
    public void AddXP_ShouldCalculateLevelCorrectly(int xp, int expectedLevel)
    {
        var user = User.Create("João", "joao@test.com", "hash");

        user.AddXP(xp);

        user.Level.Should().Be(expectedLevel);
    }
}
