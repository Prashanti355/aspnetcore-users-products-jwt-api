namespace UsersProducts.Api.Infrastructure.Seed;

public sealed class DatabaseInitializationOptions
{
    public const string SectionName = "DatabaseInitialization";

    public bool ApplyMigrations { get; init; }
}