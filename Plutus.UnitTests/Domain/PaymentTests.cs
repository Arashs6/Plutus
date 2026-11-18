using FluentAssertions;
using NSubstitute;
using Plutus.Domain;

namespace Plutus.UnitTests.Domain;

public class PaymentTests
{
    private static Money CreateSampleMoney() => new(100_000m, "IRR");

    [Fact]
    public void Constructor_WithValidMoneyAndTimeProvider_ShouldInitializePayment()
    {
        // Arrange
        var money = CreateSampleMoney();
        var fixedNow = new DateTimeOffset(2026, 11, 17, 18, 0, 0, TimeSpan.Zero);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(fixedNow);

        // Act
        var payment = new Payment(money, timeProvider);

        // Assert
        payment.Id.Should().NotBe(Guid.Empty);
        payment.Money.Should().Be(money);
        payment.CreatedAt.Should().Be(fixedNow);
    }

    [Fact]
    public void Constructor_WhenMoneyIsNull_ShouldThrowDomainException()
    {
        // Act
        var act = () => new Payment(null!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Money cannot be null.");
    }
}
