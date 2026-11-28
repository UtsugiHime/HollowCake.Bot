using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace HollowCake.Bot.NetCord.Exceptions;

public sealed class ComponentInteractionErrorHandler<TContext>(InteractionErrorHandler handler) :
    IComponentInteractionResultHandler<TContext> where TContext : IComponentInteractionContext
{
    public ValueTask HandleResultAsync(
        IExecutionResult result, 
        TContext context, 
        GatewayClient? client, 
        ILogger logger,
        IServiceProvider services)
        => handler.HandleAsync(
            result,
            context.Interaction,
            logger);
}