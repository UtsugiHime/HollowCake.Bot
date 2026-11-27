using HollowCake.Bot.NetCord.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace HollowCake.Bot.NetCord.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHollowCakeErrorHandling(
        this IServiceCollection services,
        Action<InteractionErrorOptions>? config = null)
    {
        var options = new InteractionErrorOptions();
        
        config?.Invoke(options);
        
        services.AddSingleton(options);
        services.AddSingleton<InteractionErrorHandler>();
        
        return services;
    }
}