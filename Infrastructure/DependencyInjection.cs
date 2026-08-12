using Application.Common.Interfaces;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures infrastructure services including DbContext, repositories, and services.
        /// </summary>
        public static IServiceCollection ConfigureInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            // Configure Database Context
            ConfigureDatabase(services, configuration, isDevelopment);

            // Register Repositories
            RegisterRepositories(services);

            // Register Infrastructure Services
            RegisterServices(services);

            return services;
        }

        /// <summary>
        /// Configures the database context with SQL Server.
        /// </summary>
        private static void ConfigureDatabase(
           IServiceCollection services,
           IConfiguration configuration,
           bool isDevelopment)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string 'DefaultConnection' is not configured.");
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });

                // Enable sensitive data logging in development
                if (isDevelopment)
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });
        }

        /// <summary>
        /// Registers all repository implementations.
        /// </summary>
        private static void RegisterRepositories(IServiceCollection services)
        {

            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<ITrackRepository, TrackRepository>();
            services.AddScoped<IDspRepository, DspRepository>();
        }

        /// <summary>
        /// Registers infrastructure services.
        /// </summary>
        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ILoggerService, LoggerService>();
        }

        /// <summary>
        /// Automatically applies pending EF Core migrations on startup.
        /// Use with caution in production environments.
        /// </summary>
        public static void MigrateDatabase(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<AppDbContext>>();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pendingMigrations = dbContext.Database.GetPendingMigrations();

                if (pendingMigrations.Any())
                {
                    logger?.LogInformation(
                        "Applying {Count} pending migration(s): {Migrations}",
                        pendingMigrations.Count(),
                        string.Join(", ", pendingMigrations));

                    dbContext.Database.Migrate();

                    logger?.LogInformation("Database migrated successfully.");
                }
                else
                {
                    logger?.LogInformation("Database is already up to date. No pending migrations.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error occurred while applying database migrations.");

                // Re-throw in development to catch issues early
                throw;
            }
        }

        /// <summary>
        /// Ensures the database is created. Useful for development/testing.
        /// Do not use in production - use migrations instead.
        /// </summary>
        public static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<AppDbContext>>();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (dbContext.Database.EnsureCreated())
                {
                    logger?.LogInformation("Database created successfully.");
                }
                else
                {
                    logger?.LogInformation("Database already exists.");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error occurred while ensuring database is created.");
                throw;
            }
        }
    }
}