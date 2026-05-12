using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsersProducts.Api.Domain.Entities;
using UsersProducts.Api.Domain.Enums;
using UsersProducts.Api.Infrastructure.Persistence;

namespace UsersProducts.Api.Infrastructure.Seed;

public sealed class DatabaseInitializerHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<DatabaseInitializationOptions> _databaseOptions;
    private readonly IOptions<AdminSeedOptions> _adminSeedOptions;
    private readonly ILogger<DatabaseInitializerHostedService> _logger;

    public DatabaseInitializerHostedService(
        IServiceScopeFactory scopeFactory,
        IOptions<DatabaseInitializationOptions> databaseOptions,
        IOptions<AdminSeedOptions> adminSeedOptions,
        ILogger<DatabaseInitializerHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _databaseOptions = databaseOptions;
        _adminSeedOptions = adminSeedOptions;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        if (_databaseOptions.Value.ApplyMigrations)
        {
            _logger.LogInformation("Aplicando migraciones pendientes de la base de datos.");
            await dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migraciones aplicadas correctamente.");
        }

        if (!_adminSeedOptions.Value.Enabled)
        {
            _logger.LogInformation("Seed de administrador inicial deshabilitado.");
            return;
        }

        await SeedAdminAsync(dbContext, passwordHasher, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedAdminAsync(
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken)
    {
        var options = _adminSeedOptions.Value;

        ValidateAdminSeedOptions(options);

        var adminAlreadyExists = await dbContext.Users
            .AnyAsync(user => user.Role == UserRole.Admin, cancellationToken);

        if (adminAlreadyExists)
        {
            _logger.LogInformation("Ya existe al menos un usuario administrador. No se creó un administrador inicial.");
            return;
        }

        var normalizedEmail = User.NormalizeEmail(options.Email);

        var emailAlreadyExists = await dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailAlreadyExists)
        {
            throw new InvalidOperationException(
                "El correo configurado para el administrador inicial ya existe, pero no pertenece a un administrador."
            );
        }

        var admin = new User(
            name: options.Name,
            email: normalizedEmail,
            role: UserRole.Admin
        );

        var passwordHash = passwordHasher.HashPassword(admin, options.Password);
        admin.SetPasswordHash(passwordHash);

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Administrador inicial creado correctamente con correo {AdminEmail}.",
            normalizedEmail
        );
    }

    private static void ValidateAdminSeedOptions(AdminSeedOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Name))
        {
            throw new InvalidOperationException("AdminSeed:Name no está configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.Email))
        {
            throw new InvalidOperationException("AdminSeed:Email no está configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException("AdminSeed:Password no está configurado.");
        }

        if (options.Password.Length < 8)
        {
            throw new InvalidOperationException("AdminSeed:Password debe tener al menos 8 caracteres.");
        }
    }
}