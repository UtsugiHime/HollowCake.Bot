using NetCord;
using HollowCake.Bot.Core.Exceptions;
using HollowCake.Bot.NetCord.Responses;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace HollowCake.Bot.NetCord.Exceptions;

public sealed class InteractionErrorHandler(
    InteractionErrorOptions options, ResponseService responses, EmbedFactory embeds) : 
    IApplicationCommandResultHandler<ApplicationCommandContext>, 
    IComponentInteractionResultHandler<ComponentInteractionContext>
{
    private const int AlreadyAcknowledged = 40060;
    private const int UnknownMessage = 10008;

    public ValueTask HandleResultAsync(
        IExecutionResult result, 
        ApplicationCommandContext context, 
        GatewayClient? client,
        ILogger logger, 
        IServiceProvider services) 
        => HandleAsync(result, context.Interaction, logger);

    public ValueTask HandleResultAsync(
        IExecutionResult result, 
        ComponentInteractionContext context, 
        GatewayClient? client,
        ILogger logger, 
        IServiceProvider services) 
        => HandleAsync(result, context.Interaction, logger);

    internal async ValueTask HandleAsync(IExecutionResult result, Interaction interaction, ILogger logger)
    {
        if (result is not IFailResult failure)
            return;

        var message = ResolveMessage(failure, logger);
        var embed = embeds.Error(options.Title, message);
        
        try
        {
            await RespondUnknownAsync(interaction, embed);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to respond to interaction {InteractionId}.", interaction.Id);
        }
    }

    private string ResolveMessage(IFailResult failure, ILogger logger)
    {
        if (failure is IExceptionResult exceptionResult)
        {
            var exception = exceptionResult.Exception;

            if (exception is BotException botException)
            {
                logger.Log(
                    botException is BotExternalServiceException ? LogLevel.Error : LogLevel.Warning,
                    botException,
                    "{ExceptionMessage}",
                    botException.Message);
                
                return botException.UserMessage;
            }
            logger.LogError(exception, "Unexpected interaction error.");
            return options.UnexpectedErrorMessage;
        }
        logger.LogWarning("Interaction execution failed: {FailureMessage}", failure.Message);
        return options.ExecutionFailedMessage;
    }

    private async Task RespondAsync(Interaction interaction, EmbedProperties embed)
    {
        switch (responses.GetState(interaction))
        {
            case InteractionResponseState.DeferredMessage:
                await responses.ModifyResponseAsync(interaction, embed);
                break;
            
            case InteractionResponseState.DeferredUpdate:
            case InteractionResponseState.Acknowledged:
                await responses.FollowupAsync(interaction, embed, true);
                break;
            
            case InteractionResponseState.Unknown:
            default:
                await RespondUnknownAsync(interaction, embed);
                break;
        }
    }
    
    private async Task RespondUnknownAsync(Interaction interaction, EmbedProperties embed)
    {
        try
        {
            await responses.RespondAsync(interaction, embed, true);
            return;
        }
        catch (RestException r) when (r.Error?.Code == AlreadyAcknowledged)
        {
            // Ignore, the interaction has already been acknowledged.
        }

        RestMessage? original = null;

        try
        {
            original = await responses.GetResponseAsync(interaction);
        }
        catch (RestException r) when (r.Error?.Code == UnknownMessage)
        {
            // Ignore, no original message was found.
        }

        if (original is not null && original.Flags.HasFlag(MessageFlags.Loading))
        {
            await responses.ModifyResponseAsync(interaction, embed);
            return;
        }
        
        await responses.FollowupAsync(interaction, embed, true);
    }
}