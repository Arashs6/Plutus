using FluentAssertions;
using Plutus.Domain;

namespace Plutus.UnitTests.Domain;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidAmountAndCurrency_ShouldSetPropertiesAndNormalizeCurrency()
    {
        // Arrange
        const decimal amount = 150_000m;

        // Act
        var money = new Money(amount, " irr ");

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldSucceed()
    {
        // Act
        var money = new Money(0m, "IRR");

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Constructor_WhenAmountIsNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Act
        var act = () => new Money(-1m, "IRR");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("amount");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenCurrencyIsNullOrWhiteSpace_ShouldThrowArgumentException(string? invalidCurrency)
    {
        // Act
        var act = () => new Money(1000m, invalidCurrency!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("currency");
    }

    [Fact]
    public void Zero_WithValidCurrency_ShouldReturnZeroMoneyWithNormalizedCurrency()
    {
        // Act
        var zero = Money.Zero("usd");

        // Assert
        zero.Amount.Should().Be(0m);
        zero.Currency.Should().Be("USD");
    }

    [Fact]
    public void Equality_WhenAmountAndNormalizedCurrencyAreEqual_ShouldBeEqual()
    {
        // Arrange
        var first = new Money(50_000m, "IRR");
        var second = new Money(50_000m, " irr ");

        // Act & Assert
        first.Should().Be(second);
        (first == second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());
        first.Should().NotBeSameAs(second);
    }

    [Theory]
    [InlineData(1000, "IRR", 2000, "IRR")]
    [InlineData(1000, "IRR", 1000, "USD")]
    public void Equality_WhenAmountOrCurrencyDiffers_ShouldNotBeEqual(
        decimal firstAmount,
        string firstCurrency,
        decimal secondAmount,
        string secondCurrency)
    {
        // Arrange
        var first = new Money(firstAmount, firstCurrency);
        var second = new Money(secondAmount, secondCurrency);

        // Act & Assert
        first.Should().NotBe(second);
        (first != second).Should().BeTrue();
    }
}
