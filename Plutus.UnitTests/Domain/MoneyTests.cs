using FluentAssertions;
using Plutus.Domain;

namespace Plutus.UnitTests.Domain;

public class MoneyTests
{
    #region Constructor

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
    public void Constructor_WhenAmountIsNegative_ShouldThrowDomainException()
    {
        // Act
        var act = () => new Money(-1m, "IRR");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenCurrencyIsNullOrWhiteSpace_ShouldThrowDomainException(string? invalidCurrency)
    {
        // Act
        var act = () => new Money(1000m, invalidCurrency!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currency cannot be null or whitespace.");
    }

    #endregion

    #region Zero

    [Fact]
    public void Zero_WithValidCurrency_ShouldReturnZeroMoneyWithNormalizedCurrency()
    {
        // Act
        var zero = Money.Zero("usd");

        // Assert
        zero.Amount.Should().Be(0m);
        zero.Currency.Should().Be("USD");
    }

    #endregion

    #region Equality

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

    [Fact]
    public void Equality_WhenAmountsDiffer_ShouldNotBeEqual()
    {
        // Arrange
        var first = new Money(1000m, "IRR");
        var second = new Money(2000m, "IRR");

        // Act & Assert
        first.Should().NotBe(second);
        (first != second).Should().BeTrue();
    }

    [Fact]
    public void Equality_WhenCurrenciesDiffer_ShouldNotBeEqual()
    {
        // Arrange
        var first = new Money(1000m, "IRR");
        var second = new Money(1000m, "USD");

        // Act & Assert
        first.Should().NotBe(second);
        (first != second).Should().BeTrue();
    }

    #endregion

    #region Add

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnNewMoneyWithSum()
    {
        // Arrange
        var first = new Money(10_000m, "IRR");
        var second = new Money(25_000m, "irr");

        // Act
        var result = first.Add(second);

        // Assert
        result.Should().Be(new Money(35_000m, "IRR"));
        first.Amount.Should().Be(10_000m);
    }

    [Fact]
    public void Add_WhenMoneyIsNull_ShouldThrowDomainException()
    {
        // Arrange
        var first = new Money(10_000m, "IRR");

        // Act
        var act = () => first.Add(null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Money cannot be null.");
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var irr = new Money(10_000m, "IRR");
        var usd = new Money(10m, "USD");

        // Act
        var act = () => irr.Add(usd);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currencies must match.");
    }

    #endregion

    #region Subtract

    [Fact]
    public void Subtract_WithSameCurrencyAndSufficientAmount_ShouldReturnNewMoneyWithDifference()
    {
        // Arrange
        var first = new Money(30_000m, "IRR");
        var second = new Money(12_000m, "IRR");

        // Act
        var result = first.Subtract(second);

        // Assert
        result.Should().Be(new Money(18_000m, "IRR"));
        first.Amount.Should().Be(30_000m);
    }

    [Fact]
    public void Subtract_WithEqualAmounts_ShouldReturnZeroMoney()
    {
        // Arrange
        var first = new Money(10_000m, "IRR");
        var second = new Money(10_000m, "IRR");

        // Act
        var result = first.Subtract(second);

        // Assert
        result.Should().Be(Money.Zero("IRR"));
    }

    [Fact]
    public void Subtract_WhenMoneyIsNull_ShouldThrowDomainException()
    {
        // Arrange
        var first = new Money(10_000m, "IRR");

        // Act
        var act = () => first.Subtract(null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Money cannot be null.");
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var irr = new Money(10_000m, "IRR");
        var usd = new Money(5m, "USD");

        // Act
        var act = () => irr.Subtract(usd);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currencies must match.");
    }

    [Fact]
    public void Subtract_WhenResultWouldBeNegative_ShouldThrowDomainException()
    {
        // Arrange
        var smaller = new Money(10_000m, "IRR");
        var larger = new Money(25_000m, "IRR");

        // Act
        var act = () => smaller.Subtract(larger);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Result cannot be negative.");
    }

    #endregion
}
