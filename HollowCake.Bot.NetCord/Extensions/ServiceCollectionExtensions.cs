using HollowCake.Bot.NetCord.Exceptions;
using HollowCake.Bot.NetCord.Responses;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

namespace HollowCake.Bot.NetCord.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddHollowCakeErrorHandling(Action<InteractionErrorOptions>? config = null)
        {
            var options = new InteractionErrorOptions();
            config?.Invoke(options);
            
            services.AddSingleton(options);
            services.AddSingleton<InteractionErrorHandler>();
            services.AddSingleton(typeof(ComponentInteractionErrorHandler<>));
            
            return services;
        }
        
        public IServiceCollection AddHollowCakeResponses(Action<ResponseOptions>? config = null)
        {
            var options = new ResponseOptions();
            config?.Invoke(options);
            
            services.AddSingleton(options);
            services.AddSingleton<EmbedFactory>();
            
            return services;
        }

        public IServiceCollection AddHollowCakeComponentInteractions<TInteraction, TContext>() 
            where TInteraction : ComponentInteraction 
            where TContext : IComponentInteractionContext
        {
            return services.AddComponentInteractions<TInteraction, TContext>(
                (options, provider) =>
                {
                    options.ResultHandler = provider.GetRequiredService<ComponentInteractionErrorHandler<TContext>>();
                });
        }
    }
}