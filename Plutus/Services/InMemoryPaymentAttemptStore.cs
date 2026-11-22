using System.Collections.Concurrent;
using Plutus.Application;
using Plutus.Domain;

namespace Plutus.Services;

public sealed class InMemoryPaymentAttemptStore : IPaymentAttemptStore
{
    private readonly ConcurrentDictionary<string, PaymentAttempt> _attempts = new(StringComparer.Ordinal);

    public void Save(string reference, PaymentOrder order, long amount)
    {
        _attempts[reference] = new PaymentAttempt(order, amount);
    }

    public bool TryGet(string reference, out PaymentAttempt? attempt)
    {
        return _attempts.TryGetValue(reference, out attempt);
    }
}

