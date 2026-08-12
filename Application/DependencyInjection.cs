using Application.Services; 
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void ConfigureApplication(this IServiceCollection services)
        {
            // 1. Register all FluentValidation validators located in the Application project
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<ArtistService>();
            services.AddScoped<TrackService>();
            services.AddScoped<AuthService>();

        }
    }
}