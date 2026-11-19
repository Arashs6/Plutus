# Plutus Workspace Rules

## Workflow & Scope
- This is a hands-on learning project for building a .NET 10 payment microservice with best practices.
- Stay strictly within the scope of what the user asks or has already written; do not jump ahead or add unrequested features, methods, or refactors.
- After code changes, verify using `dotnet build` only. Do not run `dotnet test`, because the user uses NCrunch to execute tests automatically.

## Domain Conventions (`Plutus.Domain`)
- Use `DomainException` (via `DomainException.ThrowIf` and `DomainException.ThrowIfNull`) consistently for domain invariant violations.
- Prefer strong encapsulation and immutability on domain types (e.g., `private init` or `private set`, read-only value object properties).

## Unit Testing Conventions (`Plutus.UnitTests`)
- Use `xUnit`, `FluentAssertions`, and `NSubstitute` (use `NSubstitute` only for real dependencies such as interfaces or `TimeProvider`, not pure value objects).
- Group tests inside test classes using `#region` / `#endregion` blocks by method or category (e.g., `Constructor`, `Zero`, `Equality`, `Add`, `Subtract`).
- Prefer DAMP readability: use separate `[Fact]` methods for distinct scenarios or important boundary conditions rather than forcing `[Theory]`/`[InlineData]` with many parameters; use `[Theory]`/`[InlineData]` when multiple simple inputs verify the same rule (e.g., `null`, `""`, `"   "`).
