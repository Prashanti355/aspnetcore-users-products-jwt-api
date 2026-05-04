namespace UsersProducts.Api.Contracts.Health;

public sealed record HealthResponse(
    string Status,
    string Service,
    string Environment,
    DateTime TimestampUtc
);