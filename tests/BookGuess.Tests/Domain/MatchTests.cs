using BookGuess.Domain.Entities;
using BookGuess.Domain.Enums;
using FluentAssertions;

namespace BookGuess.Tests.Domain;

public class MatchTests
{
    [Fact]
    public void Create_ShouldInitializeWithActiveStatusAndBaseScore()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        match.Status.Should().Be(MatchStatus.Active);
        match.Score.Should().Be(Match.BaseScore);
        match.Attempts.Should().Be(0);
        match.HintsUsed.Should().Be(0);
    }

    [Fact]
    public void RegisterIncorrectGuess_ShouldDeductScoreAndIncrementAttempts()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        match.RegisterIncorrectGuess();

        match.Attempts.Should().Be(1);
        match.Score.Should().Be(Match.BaseScore - Match.ScorePenaltyPerError);
        match.Status.Should().Be(MatchStatus.Active);
    }

    [Fact]
    public void RegisterIncorrectGuess_WhenMaxAttemptsReached_ShouldLose()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        for (int i = 0; i < Match.MaxAttempts; i++)
            match.RegisterIncorrectGuess();

        match.Status.Should().Be(MatchStatus.Lost);
        match.FinishedAt.Should().NotBeNull();
    }

    [Fact]
    public void RegisterCorrectGuess_ShouldSetWonStatus()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        match.RegisterCorrectGuess();

        match.Status.Should().Be(MatchStatus.Won);
        match.FinishedAt.Should().NotBeNull();
    }

    [Fact]
    public void UseHint_ShouldDeductScoreAndIncrementHints()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        var result = match.UseHint();

        result.Should().BeTrue();
        match.HintsUsed.Should().Be(1);
        match.Score.Should().Be(Match.BaseScore - Match.ScorePenaltyPerHint);
    }

    [Fact]
    public void UseHint_WhenMaxHintsReached_ShouldReturnFalse()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        for (int i = 0; i < Match.MaxHints; i++)
            match.UseHint();

        var result = match.UseHint();

        result.Should().BeFalse();
        match.HintsUsed.Should().Be(Match.MaxHints);
    }

    [Fact]
    public void Score_ShouldNeverGoBelowZero()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        for (int i = 0; i < 20; i++)
            match.RegisterIncorrectGuess();

        match.Score.Should().Be(0);
    }

    [Fact]
    public void CalculateXP_OnFirstAttemptNoHints_ShouldReturn175()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        match.RegisterCorrectGuess();

        match.CalculateXP().Should().Be(100 + 50 + 25);
    }

    [Fact]
    public void CalculateXP_DailyChallenge_ShouldIncludeBonus()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid(), isDaily: true);

        match.RegisterCorrectGuess();

        match.CalculateXP().Should().Be(100 + 50 + 25 + 100);
    }

    [Fact]
    public void CalculateXP_WhenLost_ShouldReturnZero()
    {
        var match = Match.Create(Guid.NewGuid(), Guid.NewGuid());

        for (int i = 0; i < Match.MaxAttempts; i++)
            match.RegisterIncorrectGuess();

        match.CalculateXP().Should().Be(0);
    }
}
